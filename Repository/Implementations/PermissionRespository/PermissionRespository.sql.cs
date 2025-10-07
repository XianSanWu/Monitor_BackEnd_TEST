using Dapper;
using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Dto.Common;
using Models.Entities;
using Models.Enums;
using Repository.Interfaces;
using System.Text;
using Utilities.Utilities;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;

namespace Repository.Implementations.PermissionRespository
{
    public partial class PermissionRespository : BaseRepository, IPermissionRespository
    {
        private void SaveFeaturePermissions(PermissionUpdateRequest updateReq)
        {
            if (updateReq?.FieldRequest == null || !updateReq.FieldRequest.Any())
                throw new ArgumentException("FieldRequest 不得為空");

            _sqlParams = new DynamicParameters();

            // 把 FieldRequest 轉成 inline table (UNION ALL SELECT)
            var srcSql = string.Join("\nUNION ALL\n",
                updateReq.FieldRequest.Select((f, i) =>
                {
                    _sqlParams.Add($"@Uuid_{i}", f.Uuid);
                    _sqlParams.Add($"@ParentUuid_{i}", f.ParentUuid);
                    _sqlParams.Add($"@Icon_{i}", f.Icon);
                    _sqlParams.Add($"@ModuleName_{i}", f.ModuleName);
                    _sqlParams.Add($"@FeatureName_{i}", f.FeatureName);
                    _sqlParams.Add($"@Title_{i}", f.Title);
                    _sqlParams.Add($"@Action_{i}", f.Action);
                    _sqlParams.Add($"@Link_{i}", f.Link);
                    _sqlParams.Add($"@BitValue_{i}", f.BitValue);
                    _sqlParams.Add($"@Sort_{i}", f.Sort);
                    _sqlParams.Add($"@IsUse_{i}", f.IsUse);
                    _sqlParams.Add($"@IsVisible_{i}", f.IsVisible);
                    _sqlParams.Add($"@UpdateAt_{i}", f.UpdateAt ?? DateTime.Now);

                    return $@"SELECT 
                @Uuid_{i} Uuid,
                @ParentUuid_{i} ParentUuid,
                @Icon_{i} Icon,
                @ModuleName_{i} ModuleName,
                @FeatureName_{i} FeatureName,
                @Title_{i} Title,
                @Action_{i} Action,
                @Link_{i} Link,
                @BitValue_{i} BitValue,
                @Sort_{i} Sort,
                @IsUse_{i} IsUse,
                @IsVisible_{i} IsVisible,
                @UpdateAt_{i} UpdateAt";
                })
            );

            // MERGE SQL → 用 Uuid 判斷新增或更新
            _sqlStr = new StringBuilder($@"
MERGE FeaturePermissions AS target
USING (
    {srcSql}
) AS src
ON target.Uuid = src.Uuid
WHEN MATCHED 
    AND (
        ISNULL(target.ParentUuid,'') <> ISNULL(src.ParentUuid,'') OR
        ISNULL(target.Icon,'') <> ISNULL(src.Icon,'') OR
        ISNULL(target.ModuleName,'') <> ISNULL(src.ModuleName,'') OR
        ISNULL(target.FeatureName,'') <> ISNULL(src.FeatureName,'') OR
        ISNULL(target.Title,'') <> ISNULL(src.Title,'') OR
        ISNULL(target.Action,'') <> ISNULL(src.Action,'') OR
        ISNULL(target.Link,'') <> ISNULL(src.Link,'') OR
        target.BitValue <> src.BitValue OR
        ISNULL(target.Sort,0) <> ISNULL(src.Sort,0) OR
        ISNULL(target.IsUse,0) <> ISNULL(src.IsUse,0) OR
        ISNULL(target.IsVisible,0) <> ISNULL(src.IsVisible,0)
    )
THEN
    UPDATE SET 
        ParentUuid = src.ParentUuid,
        Icon = src.Icon,
        ModuleName = src.ModuleName,
        FeatureName = src.FeatureName,
        Title = src.Title,
        Action = src.Action,
        Link = src.Link,
        BitValue = src.BitValue,
        Sort = src.Sort,
        IsUse = src.IsUse,
        IsVisible = src.IsVisible,
        UpdateAt = src.UpdateAt
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Uuid, ParentUuid, Icon, ModuleName, FeatureName, Title, Action, Link, BitValue, Sort, IsUse, IsVisible, UpdateAt)
    VALUES (src.Uuid, src.ParentUuid, src.Icon, src.ModuleName, src.FeatureName, src.Title, src.Action, src.Link, src.BitValue, src.Sort, src.IsUse, src.IsVisible, src.UpdateAt);
");

        }


        private void CheckUpdateUser(UserUpdateRequest updateReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr.AppendLine(" SELECT * FROM Users WITH(NOLOCK) WHERE 1=1 AND UserName = @UserName ");

            _sqlParams = new DynamicParameters();
            if (updateReq.FieldRequest == null)
                throw new InvalidOperationException("沒有任何欄位可更新");

            _sqlParams.Add("@UserName", updateReq.FieldRequest.UserName);
        }

        private void SaveUser(UserUpdateRequest updateReq)
        {
            _sqlStr = new StringBuilder();
            _sqlParams = new DynamicParameters();

            // 先組成 UPDATE SET 部分
            var setClauses = new List<string>();
            var paramNamesForInsert = new List<string>(); // 保持 INSERT VALUES 的順序

            var fieldProps = Reflection.GetModelPropertiesWithValues(updateReq.FieldRequest);
            foreach (var prop in fieldProps)
            {
                var columnName = prop.Key;
                var (type, value) = prop.Value;

                var paramKey = $"@{columnName}";
                setClauses.Add($"{columnName} = {paramKey}");
                paramNamesForInsert.Add(paramKey);

                _sqlParams.Add(paramKey, value);
            }

            if (!setClauses.Any())
                throw new InvalidOperationException("沒有任何欄位可更新");

            // 組成條件部分 (使用原本 ConditionRequest 群組邏輯)
            var whereGroups = new List<string>();
            if (updateReq.ConditionRequest != null && updateReq.ConditionRequest.Any())
            {
                int groupIndex = 0;
                foreach (var group in updateReq.ConditionRequest)
                {
                    var groupConditions = new List<string>();
                    var props = Reflection.GetModelPropertiesWithValues(group);

                    foreach (var prop in props)
                    {
                        var columnName = prop.Key;
                        var (type, value) = prop.Value;

                        if (value is not FieldWithMetadataModel meta ||
                            string.IsNullOrWhiteSpace(meta.MathSymbol?.ToString()) ||
                            meta.Value == null ||
                            string.IsNullOrWhiteSpace(meta.Value?.ToString()))
                            continue;

                        switch (meta.MathSymbol.ToUpper())
                        {
                            case "IN":
                                if (meta.Value is IEnumerable<object> list && meta.Value is not string)
                                {
                                    var placeholders = new List<string>();
                                    int index = 0;
                                    foreach (var item in list)
                                    {
                                        var paramName = $"@cond_{groupIndex}_{columnName}_{index++}";
                                        placeholders.Add(paramName);
                                        _sqlParams.Add(paramName, item);
                                    }
                                    if (placeholders.Count > 0)
                                        groupConditions.Add($"{columnName} IN ({string.Join(", ", placeholders)})");
                                }
                                break;

                            case "LIKE":
                                var likeParam = $"@cond_{groupIndex}_{columnName}";
                                groupConditions.Add($"{columnName} LIKE {likeParam}");
                                _sqlParams.Add(likeParam, $"%{meta.Value}%");
                                break;

                            default:
                                var paramKey = $"@cond_{groupIndex}_{columnName}";
                                groupConditions.Add($"{columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {paramKey}");
                                _sqlParams.Add(paramKey, meta.Value);
                                break;
                        }
                    }

                    if (groupConditions.Count > 0)
                    {
                        string insideLogicOperator = group.InsideLogicOperator == LogicOperatorEnum.None
                            ? "AND"
                            : group.InsideLogicOperator.ToString();

                        whereGroups.Add("(" + string.Join($" {insideLogicOperator} ", groupConditions) + ")");

                        if (group.GroupLogicOperator != LogicOperatorEnum.None)
                            whereGroups.Add(group.GroupLogicOperator.ToString());
                    }

                    groupIndex++;
                }

                // 移除最後一個多餘的 AND / OR
                if (whereGroups.Count > 0)
                {
                    var last = whereGroups[^1].Trim().ToUpper();
                    if (last == "AND" || last == "OR")
                        whereGroups.RemoveAt(whereGroups.Count - 1);
                }
            }

            // 如果 ConditionRequest 是空的 → 直接新增
            if (!whereGroups.Any())
            {
                _sqlStr.AppendLine(@$"
INSERT INTO Users ({string.Join(",", fieldProps.Keys)})
VALUES ({string.Join(",", paramNamesForInsert)})
");
            }
            else
            {
                // 有條件 → 用 MERGE 做 Upsert
                _sqlStr.AppendLine(@$"
MERGE INTO Users AS target
USING (SELECT 1 AS dummy) AS source
    ON {string.Join(" ", whereGroups)}
WHEN MATCHED THEN
    UPDATE SET {string.Join(",", setClauses)}
WHEN NOT MATCHED THEN
    INSERT ({string.Join(",", fieldProps.Keys)})
    VALUES ({string.Join(",", paramNamesForInsert)})
;");
            }
        }


        private void IsUseUser(UserUpdateRequest updateReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@" UPDATE Users SET ");

            _sqlParams = new DynamicParameters();
            #region 處理欄位更新

            var columnsWithValues = Reflection.GetModelPropertiesWithValues(updateReq.FieldRequest);
            var tempList = new List<string>();

            #region 排除不可被更新欄位（例如主鍵）
            var excludeFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id","Uuid",
                //"CreateAt","UpdateAt","JourneyCreateAt","JourneyUpdateAt","GroupSendCreateAt","GroupSendUpdateAt"
            };
            #endregion

            #region 組合要更新的欄位
            foreach (var column in columnsWithValues)
            {
                var columnName = column.Key;
                var (type, value) = column.Value;


                DateTime? date = null;
                try
                {
                    if (DateTime.TryParse(value?.ToString(), out DateTime parsedDate))
                    {
                        date = parsedDate;
                    }
                }
                catch (Exception)
                {
                    //不理會
                }

                if (excludeFields.Contains(columnName) ||
                    value == null ||
                    string.IsNullOrWhiteSpace(value?.ToString()) ||
                    (date)?.ToString("yyyy/MM/dd") == DateTime.MinValue.ToString("yyyy/MM/dd"))
                    continue;

                var paramName = $"@{columnName}";

                #region 處理特殊型別（可自訂）
                // 處理特殊型別（可自訂）
                object? dbValue = value;

                //if (value == null)
                //{
                //    dbValue = DBNull.Value;
                //}
                //else if (type == typeof(bool))
                //{
                //    dbValue = (bool)value ? 1 : 0; // 儲存成 bit/int
                //}
                //else if (type.IsEnum)
                //{
                //    dbValue = value.ToString(); // 儲存 enum 名稱，也可改成數值
                //}
                //else if (type == typeof(DateTime) || type == typeof(DateTime?))
                //{
                //    // 可加格式化或轉 UTC，看需求
                //    dbValue = value;
                //}
                #endregion

                if (value is null)
                {
                    continue;
                }

                if (string.Equals("IsUse", columnName, StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    value = ((bool)value) ? 1 : 0;
                }

                tempList.Add($"{columnName} = {paramName}");
                _sqlParams.Add(paramName, dbValue);
            }

            _sqlStr?.Append(string.Join(", ", tempList));
            #endregion

            #endregion

            #region 處理欄位條件（多組 OR 群組，每組內 AND 條件）
            if (updateReq.ConditionRequest != null && updateReq.ConditionRequest.Any())
            {
                var whereGroups = new List<string>();
                int groupIndex = 0;

                foreach (var group in updateReq.ConditionRequest)
                {
                    var groupConditions = new List<string>();

                    var props = Reflection.GetModelPropertiesWithValues(group);
                    foreach (var prop in props)
                    {
                        var columnName = prop.Key;
                        var (type, value) = prop.Value;

                        if (value is not FieldWithMetadataModel meta || // 確保 meta 是正確的型別
                            string.IsNullOrWhiteSpace(meta.MathSymbol?.ToString()) ||
                            meta.Value == null ||
                            string.IsNullOrWhiteSpace(meta.Value?.ToString()))
                            continue;

                        switch (meta.MathSymbol.ToUpper())
                        {
                            case "MAX":
                            case "MIN":
                                var mathParamName = $"@cond_{groupIndex}_{columnName} ";
                                groupConditions.Add($"{columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol}({mathParamName}) ");
                                break;

                            case "IN":
                                // 特別排除 string 因為 string 也是 IEnumerable
                                if (meta.Value is IEnumerable<object> list && meta.Value is not string)
                                {
                                    var placeholders = new List<string>();
                                    int index = 0;

                                    foreach (var item in list)
                                    {
                                        var paramName = $"@cond_{groupIndex}_{columnName}_{index++}";
                                        placeholders.Add(paramName);
                                        _sqlParams?.Add(paramName, item);
                                    }

                                    if (placeholders.Count > 0)
                                    {
                                        groupConditions.Add($" {columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} ({string.Join(", ", placeholders)}) ");
                                    }
                                }
                                break;

                            case "LIKE":
                                var likeParamName = $"@cond_{groupIndex}_{columnName} ";
                                if (columnName.EndsWith("At", StringComparison.OrdinalIgnoreCase))
                                {
                                    groupConditions.Add($" CONVERT(VARCHAR, {columnName}, 121) {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {likeParamName} ");
                                }
                                else
                                {
                                    groupConditions.Add($"{columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {likeParamName} ");
                                }

                                _sqlParams?.Add(likeParamName, $"%{value}%");
                                break;

                            default:
                                var paramKey = $"@cond_{groupIndex}_{columnName} ";
                                groupConditions.Add($" {columnName} {MathSymbolEnum.FromName(meta.MathSymbol)?.Symbol} {paramKey} ");
                                _sqlParams?.Add(paramKey, meta.Value); // 使用 meta.Value
                                break;
                        }
                    }

                    if (groupConditions.Count > 0)
                    {
                        LogicOperatorEnum opi = group.InsideLogicOperator;
                        string insideLogicOperator = opi == LogicOperatorEnum.None ? string.Empty : opi.ToString();

                        whereGroups.Add(" (" + string.Join($" {insideLogicOperator} ", groupConditions) + ") ");

                        LogicOperatorEnum opg = group.GroupLogicOperator;
                        string groupLogicOperator = opg == LogicOperatorEnum.None ? string.Empty : opg.ToString();
                        whereGroups.Add($" {groupLogicOperator} ");
                    }

                    groupIndex++;
                }

                if (whereGroups.Count > 0)
                {
                    // 移除最後一個如果是 "AND" 或 "OR"
                    var last = whereGroups[^1].Trim().ToUpper();
                    if (last == LogicOperatorEnum.AND.ToString() || last == LogicOperatorEnum.OR.ToString())
                    {
                        whereGroups.RemoveAt(whereGroups.Count - 1);
                    }

                    _sqlStr?.Append(" WHERE 1=1 AND ");
                    _sqlStr?.Append(string.Join(string.Empty, whereGroups));
                }

            }

            #endregion

        }

        private void GetUserList(UserSearchListRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@" SELECT * FROM Users WITH(NOLOCK) WHERE 1=1 ");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            foreach (var column in columnsWithValues)
            {
                var column_key = column.Key;
                var column_Value = column.Value;
                if (column_Value is null)
                {
                    continue;
                }

                if (string.Equals("IsUse", column_key, StringComparison.OrdinalIgnoreCase) && column.Value is not null)
                {
                    column_Value = ((bool)column.Value) ? 1 : 0;
                }

                AppendFilterConditionEquals(column_key, column_Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<UserEntity>();

            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    var filter_key = filter.Key;

                    AppendFilterCondition(filter_key, filter.Value, validColumns);
                }
            }
            #endregion


            #region  設定SQL排序
            if (searchReq.SortModel != null &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Value) &&
                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                var SortKey = searchReq.SortModel.Key;

                _sqlOrderByStr = $" ORDER BY {SortKey} {searchReq.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY Id DESC ";
            }
            #endregion

        }

        //        private void QueryAllPermissionSql(PermissionSearchListRequest searchReq)
        //        {
        //            _sqlStr = new StringBuilder();
        //            _sqlStr?.Append(@"
        //SELECT p.*, m.Name AS ModuleName
        //FROM Permissions p
        //JOIN Modules m ON p.ModuleId = m.ModuleId
        //WHERE p.IsDeleted = 0 AND m.IsDeleted = 0
        //");

        //            _sqlParams = new DynamicParameters();


        //            #region  處理 FilterModel Grid (模糊查詢)
        //            var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

        //            if (searchReq.FilterModel != null)
        //            {
        //                foreach (var filter in searchReq.FilterModel)
        //                {
        //                    var filter_key = filter.Key;

        //                    AppendFilterCondition(filter_key, filter.Value, validColumns);
        //                }
        //            }
        //            #endregion


        //            #region  設定SQL排序
        //            if (searchReq.SortModel != null &&
        //                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
        //                !string.IsNullOrWhiteSpace(searchReq.SortModel.Value) &&
        //                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
        //                )
        //            {
        //                var SortKey = searchReq.SortModel.Key;

        //                _sqlOrderByStr = $" ORDER BY {SortKey} {searchReq.SortModel.Value} ";
        //            }
        //            else
        //            {
        //                _sqlOrderByStr = $" ORDER BY SN DESC ";
        //            }
        //            #endregion

        //        }
        private void GetBitValue(string module, string feature, string action)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT BitValue 
            FROM FeaturePermissions 
                WITH(NOLOCK)
            WHERE 1=1
              AND ModuleName = @ModuleName 
              AND FeatureName = @FeatureName 
              AND Action = @Action
            ");

            _sqlParams = new DynamicParameters();
            _sqlParams?.Add($"@ModuleName", module);
            _sqlParams?.Add($"@FeatureName", feature);
            _sqlParams?.Add($"@Action", action);
        }

        private void GetPermissions(PermissionSearchListRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT * FROM FeaturePermissions
                WITH(NOLOCK)
            WHERE 1=1
            ");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            if (searchReq.FieldModel != null)
            {
                var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

                foreach (var column in columnsWithValues)
                {
                    var column_key = column.Key;
                    var column_Value = column.Value;
                    if (column_Value is null)
                    {
                        continue;
                    }

                    if (string.Equals("IsUse", column_key, StringComparison.OrdinalIgnoreCase) && column.Value is not null)
                    {
                        column_Value = ((bool)column.Value) ? 1 : 0;
                    }

                    AppendFilterConditionEquals(column_key, column_Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
                }
            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<FeaturePermissionEntity>();

            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    var filter_key = filter.Key;

                    AppendFilterCondition(filter_key, filter.Value, validColumns);
                }
            }
            #endregion

            #region  設定SQL排序
            if (searchReq.SortModel != null &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Value) &&
                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                var SortKey = searchReq.SortModel.Key;

                _sqlOrderByStr = $" ORDER BY {SortKey} {searchReq.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY BitValue,Sort,ModuleName ";
            }
            #endregion
        }

        private void GetUserPermissions(string? userId = null, string? userName = null, bool? isUse = true)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT * FROM FeaturePermissions
                WITH(NOLOCK)
            WHERE 1=1
            AND IsUse = 1 --此為功能啟用(誤刪)
            ");

            var custSql = string.Empty;
            _sqlParams = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                custSql = " AND Uuid = @Uuid ";
                _sqlParams?.Add($"@Uuid", userId);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                custSql += " AND UserName = @UserName ";
                _sqlParams?.Add($"@UserName", userName);
            }
            custSql += " AND IsUse = @IsUse ";
            _sqlParams?.Add($"@IsUse", ((bool)(isUse ?? true)) ? 1 : 0);

            _sqlStr?.Append(@$"
                AND BitValue & (
                    SELECT FeatureMask FROM Users
                        WITH(NOLOCK)
                    WHERE 1=1
                    {custSql}
                ) > 0
                ");

        }

        private void GetUserPermissionsMenu(string? tokenUuid = "")
        {
            if (string.IsNullOrWhiteSpace(tokenUuid))
            {
                throw new InvalidOperationException("沒有欄位參數TokenUuid");
            }

            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
-- 先取得使用者 FeatureMask
WITH UserFeature AS (
    SELECT TOP 1 u.FeatureMask
    FROM Users u WITH(NOLOCK)
    INNER JOIN UserTokens ut WITH(NOLOCK) ON ut.UserId = u.Uuid
    WHERE u.IsUse = 1
      AND ut.Uuid = @TokenUuid
)
-- 取得直接符合功能權限的功能
, DirectFeatures AS (
    SELECT fp.*
    FROM FeaturePermissions fp
    CROSS JOIN UserFeature uf
    WHERE fp.IsUse = 1
      AND fp.BitValue & uf.FeatureMask > 0
)
-- 取得父節點
, ParentFeatures AS (
    SELECT DISTINCT fpParent.*
    FROM FeaturePermissions fpParent
    INNER JOIN DirectFeatures df ON df.ParentUuid = fpParent.Uuid
    WHERE fpParent.IsUse = 1
)
-- 最終結果
SELECT *
FROM DirectFeatures
UNION
SELECT *
FROM ParentFeatures
ORDER BY Uuid
");

            var custSql = string.Empty;
            _sqlParams = new DynamicParameters();
            _sqlParams?.Add($"@TokenUuid", tokenUuid);
        }

        public void GetUserByUserName(string userName)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT *
            FROM Users
                WITH(NOLOCK)
            WHERE UserName = @UserName
            ");

            _sqlParams = new DynamicParameters();
            _sqlParams?.Add($"@UserName", userName);
        }

        public void GetUserByUserId(string userId)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT *
            FROM Users
                WITH(NOLOCK)
            WHERE Uuid = @UserId
            ");

            _sqlParams = new DynamicParameters();
            _sqlParams?.Add($"@UserId", userId);
        }
        
    }
}
