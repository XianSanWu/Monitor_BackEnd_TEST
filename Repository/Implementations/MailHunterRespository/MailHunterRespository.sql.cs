﻿using Dapper;
using Models.Entities;
using Models.Enums;
using System.Text;
using Utilities.Utilities;
using static Models.Dto.Requests.MailHunterRequest;

namespace Repository.Implementations.MailHunterRespository
{
    public partial class MailHunterRespository
    {
        private readonly string _Format = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        private void QueryProjectMailCountSql(MailHunterSearchListRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlWithStr = new StringBuilder();
            _sqlWithStr?.Append(@"
WITH Months AS (
    SELECT 1 AS month UNION ALL
    SELECT 2 UNION ALL
    SELECT 3 UNION ALL
    SELECT 4 UNION ALL
    SELECT 5 UNION ALL
    SELECT 6 UNION ALL
    SELECT 7 UNION ALL
    SELECT 8 UNION ALL
    SELECT 9 UNION ALL
    SELECT 10 UNION ALL
    SELECT 11 UNION ALL
    SELECT 12
)
");

            var departmentCondition =
                DepartmentEnum.CDP01.ToString().Equals(searchReq?.FieldModel?.Department, StringComparison.OrdinalIgnoreCase) ?
        " AND p.project_category_code = @Department " :
        " AND p.project_category_code <> @Department ";
        _sqlStr = new StringBuilder();
            _sqlStr?.Append($@"
SELECT *
FROM (
    SELECT  
        YEAR(p.project_send_date) AS year,
        CAST(MONTH(p.project_send_date) AS NVARCHAR) AS month,
        COUNT(p.project_id) AS ProjectCount,
        SUM(ISNULL(p.project_origin_total_user, 0)) AS ProjectOriginTotalUser,
        MONTH(p.project_send_date) AS MonthSort
    FROM app_mh_project p
    WITH (NOLOCK)
    WHERE 1=1
        AND p.project_send_date >= @StartDate  
        AND p.project_send_date <= @EndDate
       {departmentCondition}
    GROUP BY 
        YEAR(p.project_send_date),
        MONTH(p.project_send_date)
    
    UNION ALL
    
    -- 全年總數（實際是區間內的總數）
    SELECT  
        YEAR(p.project_send_date) AS year,
        N'區間總數' AS month,
        COUNT(p.project_id) AS ProjectCount,
        SUM(ISNULL(p.project_origin_total_user, 0)) AS ProjectOriginTotalUser,
        99 AS MonthSort
    FROM app_mh_project p
    WITH (NOLOCK)
    WHERE 1=1
        AND p.project_send_date >= @StartDate  
        AND p.project_send_date <= @EndDate
       {departmentCondition}
    GROUP BY 
        YEAR(p.project_send_date)
) amp
WHERE 1=1 
");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (寫死查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq?.FieldModel);

            foreach (var column in columnsWithValues)
            {
                if ("Department".Equals(column.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var columnValue = column.Value;
                    columnValue = DepartmentEnum.CDP01.ToString();

                    _sqlParams?.Add($"@Department", $"%{columnValue}%");
                }

                if ("StartDate".Equals(column.Key, StringComparison.OrdinalIgnoreCase))
                {
                    string? columnValue = column.Value is DateTime dt ? dt.ToString(_Format) : column.Value?.ToString();

                    _sqlParams?.Add($"@StartDate", $"{columnValue}");
                }

                if ("EndDate".Equals(column.Key, StringComparison.OrdinalIgnoreCase))
                {
                    string? columnValue = column.Value is DateTime dt ? dt.ToString(_Format) : column.Value?.ToString();
                    _sqlParams?.Add($"@EndDate", $"{columnValue}");
                }

            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<ProjectMailCountEnyity>();

            if (searchReq?.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    AppendFilterCondition($"amp.{filter.Key}", filter.Value, validColumns);
                }
            }
            #endregion

            #region  設定SQL排序
            if (searchReq?.SortModel != null &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Value) &&
                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                if ("month".Equals(searchReq.SortModel.Key, StringComparison.OrdinalIgnoreCase))
                {
                    searchReq.SortModel.Key = "monthSort";
                }

                _sqlOrderByStr = $" ORDER BY amp.{searchReq.SortModel.Key} {searchReq.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY amp.year, amp.monthSort ";
            }
            #endregion

        }
    }
}
