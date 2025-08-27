using Dapper;
using Models.Dto.Common;
using Models.Entities;
using Models.Enums;
using Repository.Interfaces;
using System.Text;
using Utilities.Utilities;
using static Models.Dto.Requests.UserRequest;

namespace Repository.Implementations.PermissionRespository
{
    public partial class PermissionRespository : BaseRepository, IPermissionRespository
    {

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

        private void GetPermissions()
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT * FROM FeaturePermissions
                WITH(NOLOCK)
            WHERE 1=1
            AND IsUse = 1
            ");
        }

        private void GetUserPermissions(string? userId = null, bool? isUse = true)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
            SELECT * FROM FeaturePermissions
                WITH(NOLOCK)
            WHERE 1=1
            AND IsUse = 1
            ");

            var custSql = string.Empty;
            _sqlParams = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                custSql = " AND Uuid = @Uuid ";
                _sqlParams?.Add($"@Uuid", userId);
            }

            custSql += " AND IsUse = @IsUse ";
            _sqlParams?.Add($"@IsUse", isUse);

            _sqlStr?.Append(@$"
                AND BitValue & (
                    SELECT FeatureMask FROM Users
                        WITH(NOLOCK)
                    WHERE 1=1
                    {custSql}
                ) > 0
                ");


        }

        public void GetUser(string userName)
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
    }
}
