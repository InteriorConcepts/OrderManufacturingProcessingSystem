using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SCH = SQL_And_Config_Handler;

namespace OMPS
{
    public interface ISqlMethods
    {
        public Task<List<Dictionary<string, object?>>?> GetSql(string templateName, object[] fields, object[] filters, Dictionary<string, string> @params, int limit = 0);
        public Task<List<Dictionary<string, object?>>?> GetRecentOrders(int limit = 0);
        public Task<List<Dictionary<string, object?>>?> GetOrderColorSet(string job, int limit = 0);
        public Task<List<Dictionary<string, object?>>?> GetOrderData(string job, int limit = 0);
        public Task<List<Dictionary<string, object?>>?> GetJobOrderLines(string job, int limit = 0);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SqlInfoAttribute : Attribute
    {
        public string Name { get; }
        public string[] Fields { get; }
        public SqlInfoAttribute(string name, string[]? fields = null)
        {
            this.Name = name;
            this.Fields = fields ?? ["*"];
        }
    }

    public class SqlMethods: ISqlMethods
    {
        public async Task<Dictionary<string, Dictionary<string, object>>?> GetTypes(string tableName)
        {
            var res = await SCH.SQLDatabaseConnection.QueryOldCRM(@$"
                select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, 
                       NUMERIC_PRECISION, DATETIME_PRECISION, 
                       IS_NULLABLE, ORDINAL_POSITION 
                from INFORMATION_SCHEMA.COLUMNS
                where TABLE_NAME='{tableName}'
                ORDER by ORDINAL_POSITION asc"
            );
            var typeInfo = res.ToDictionary(d => $"{d["COLUMN_NAME"] ?? "--"}", d => d) ?? [];
            return typeInfo;
        }

        public async Task<List<Dictionary<string, object?>>?> GetSql(string templateName, object[] fields, object[] filters, Dictionary<string, string> @params, int limit = 0)
        {
            if (templateName is "" or null)
            {
                return null;
            }
            try
            {
                // Attempt to load Template by name
                var t = SCH.SqlTemplateHandler.LoadFromName(templateName);
                // Loading failed or resulted in Template being null
                // Return null
                if (t.Item1 is false || t.Item2 is null)
                {
                    return null;
                }
                bool AcceptsFields = t.Item2.ParamsDict.ContainsKey("fields");
                // Field '*' provided, include all fields
                if (fields.Length is 1 && fields[0] is "*")
                {
                    fields = [.. t.Item2.DataHeaders];
                } else
                {
                    // Only valid fields
                    object[] existingFields = [..
                        fields.
                            Where(f => t.Item2.DataHeaders.Contains(f))
                    ];
                    // No valid fields, return null
                    if (existingFields.Length is 0)
                    {
                        return null;
                    }
                }
                // Default limit to 20, otherwise provided limit value
                var limitStr = (limit is 0 ? "TOP 20" : $"TOP {limit}");
                // Default filter (essentially no filter)
                var filtersStr = "0 = 0";
                if (filters.Length is not 0)
                {
                    // Combine filters
                    filtersStr = String.Join(
                        " AND ",
                        filters.
                            Where(f => f.ToString()?.Trim().Length is not 0).
                            Select(f => $"({f})")
                    );
                }
                // Program handled params
                Dictionary<string, string> handledParams = new()
                {
                    { "limit", limitStr },
                    { "filters", filtersStr }
                };
                // Merge handled with specific per-template params
                var mergedParams =
                    new List<Dictionary<string, string>>() { handledParams, @params }.
                        SelectMany(dict => dict).
                        ToDictionary();
                // Make query using params
                var resp = await t.Item2.AsyncQuery(mergedParams);
                if (resp is null)
                {
                    return null;
                }
                // Whether template has field param
                if (AcceptsFields is false)
                {
                    // Iterate rows returned to slim down data- only needed
                    //  when Sql Template doesn't have a fields param and
                    //  the sql query doesn't filter by itself
                    List<Dictionary<string, object?>> ret = [];
                    for (int i = 0; i < resp.Count; i++)
                    {
                        Dictionary<string, object?> temp = [];
                        // Get value of specified fields from row
                        foreach (var k in fields)
                        {
                            resp[i].TryGetValue($"{k}", out object? value);
                            Debug.WriteLine(value?.GetType().Name ?? "<null>");
                            temp.Add($"{k}", value);
                        }
                        ret.Add(temp);
                    }
                    return ret;
                } else
                {
                    // Directly map the sql result to List of value arrays (object[])
                    return resp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }

        private static readonly Regex AsyncMethodNameFormat = new Regex(@"^<(?<method_name>\w+)>");


        [SqlInfoAttribute(
            name: "OrderColorSet",
            fields: ["ColorSetID", "OppOrderID", "OpportunityID", "QuoteID",
                     "OrderNumber", "OrderDate", "LineNumber", "SupplyOrderRef",
                     "CreationDate", "ChangeDate", "NetProduct"]
            )
        ]
        public async Task<List<Dictionary<string, object?>>?> GetRecentOrders(int limit)
        {
            try
            {
                //var attr = typeof(SqlMethods).GetMethod("GetRecentOrders").ReflectedType?.GetCustomAttribute<SqlInfoAttribute>();
                return await GetSql(
                    "OrderColorSet",
                    ["ColorSetID", "OppOrderID", "OpportunityID", "QuoteID",
                     "OrderNumber", "OrderDate", "LineNumber", "SupplyOrderRef",
                     "CreationDate", "ChangeDate", "NetProduct"],
                    [$"GETDATE() - CreationDate < 14"],
                    [],
                    limit
                );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
            return null;
        }

        [SqlInfoAttribute(name: "OrderColorSet")]
        public async Task<List<Dictionary<string, object?>>?> GetOrderColorSet(string job, int limit)
        {
            try
            {
                //var attr = MethodBase.GetCurrentMethod()?.GetCustomAttribute<SqlInfoAttribute>();
                return await GetSql(
                    "OrderColorSet",
                    ["*"],
                    [$"SupplyOrderRef='{job}'"],
                    [],
                    limit
                );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
            return null;
        }

        [SqlInfoAttribute(name: "JobData_Express")]
        public async Task<List<Dictionary<string, object?>>?> GetOrderData(string job, int limit)
        {
            try
            {
                //var attr = MethodBase.GetCurrentMethod()?.GetCustomAttribute<SqlInfoAttribute>();
                return await GetSql(
                    "JobData_Express",
                    ["*"],
                    [$"JobNbr='{job}'"],
                    new() { { "job-num", job } },
                    limit
                );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
            return null;
        }

        [SqlInfoAttribute("JobLineItems")]
        public async Task<List<Dictionary<string, object?>>?> GetJobOrderLines(string job, int limit)
        {
            try
            {
                //var attr = MethodBase.GetCurrentMethod()?.GetCustomAttribute<SqlInfoAttribute>();
                return await GetSql(
                    "JobLineItems",
                    ["*"],
                    [$"JobNbr='{job}'"],
                    new() { },
                    limit
                );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
            return null;
        }
    }

}
