using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json.Linq;
using SqlKata;

[assembly: InternalsVisibleTo("Datory.Data.Tests")]

namespace Datory.Utils
{
    internal static partial class RepositoryUtils
    {
        public static async Task<int> InsertObjectAsync<T>(IDatabase database, string tableName, IEnumerable<TableColumn> tableColumns, IRedis redis, T dataInfo, Query query = null) where T : Entity
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.CreatedDate = DateTime.Now;
            dataInfo.LastModifiedDate = DateTime.Now;

            var identityInsert = false;
            if (dataInfo.Id > 0)
            {
                if (query != null)
                {
                    var identityCondition = query.GetOneComponent<BasicCondition>("identity");
                    if (identityCondition != null)
                    {
                        identityInsert = true;
                    }
                    else
                    {
                        dataInfo.Id = 0;
                    }
                }
                else
                {
                    dataInfo.Id = 0;
                }
            }

            var dictionary = new Dictionary<string, object>();
            foreach (var tableColumn in tableColumns)
            {
                if (!identityInsert && tableColumn.IsIdentity) continue;
                
                var value = ValueUtils.GetSqlValue(dataInfo, tableColumn);
                dictionary[tableColumn.AttributeName] = value;
            }

            var xQuery = NewQuery(tableName, query);
            xQuery.AsInsert(dictionary, !identityInsert);
            var compileInfo = await CompileAsync(database, tableName, redis, xQuery);

            if (identityInsert && database.DatabaseType == DatabaseType.SqlServer)
            {
                compileInfo.Sql = $@"
SET IDENTITY_INSERT {database.GetQuotedIdentifier(tableName)} ON
{compileInfo.Sql}
SET IDENTITY_INSERT {database.GetQuotedIdentifier(tableName)} OFF
";
            }

            using (var connection = database.GetConnection())
            {
                if (identityInsert)
                {
                    await connection.ExecuteAsync(compileInfo.Sql, compileInfo.NamedBindings);
                }
                else
                {
                    dataInfo.Id = await connection.QueryFirstAsync<int>(compileInfo.Sql, compileInfo.NamedBindings);
                }
            }

            return dataInfo.Id;
        }

        public static async Task BulkInsertAsync<T>(IDatabase database, string tableName, List<TableColumn> tableColumns, IEnumerable<T> items) where T : Entity
        {
            var objList = new List<IDictionary<string, object>>();
            foreach (var item in items)
            {
                objList.Add(item.ToDictionary());
            }
            await BulkInsertAsync(database, tableName, tableColumns, objList);
        }

        public static async Task BulkInsertAsync(IDatabase database, string tableName, List<TableColumn> tableColumns, IEnumerable<JObject> items)
        {
            var objList = new List<IDictionary<string, object>>();
            foreach (var item in items)
            {
                objList.Add(JsonGetDictionaryIgnoreCase(item));
            }
            await BulkInsertAsync(database, tableName, tableColumns, objList);
        }

        public static async Task BulkInsertAsync(IDatabase database, string tableName, List<TableColumn> tableColumns, IEnumerable<IDictionary<string, object>> items)
        {
            var columnNames = new StringBuilder();
            foreach (var tableColumn in tableColumns)
            {
                columnNames.Append($"{database.GetQuotedIdentifier(tableColumn.AttributeName)},");
            }
            if (columnNames.Length > 0) columnNames.Length -= 1;

            var valuesList = new List<string>();
            var parameterList = new DynamicParameters();
            var parameterCount = 0;
            var index = 0;
            foreach (var dict in items)
            {
                index++;
                var values = new StringBuilder();
                foreach (var tableColumn in tableColumns)
                {
                    if (string.IsNullOrEmpty(tableColumn?.AttributeName)) continue;

                    object val;
                    dict.TryGetValue(tableColumn.AttributeName, out val);

                    if (tableColumn.DataType == DataType.Integer)
                    {
                        if (val == null) val = 0;
                        values.Append($"{Convert.ToInt32(val)},");
                    }
                    else if (tableColumn.DataType == DataType.Decimal)
                    {
                        if (val == null) val = 0;
                        values.Append($"{Convert.ToDecimal(val)},");
                    }
                    else if (tableColumn.DataType == DataType.Boolean)
                    {
                        var paramName = $"@{tableColumn.AttributeName}_{index}";
                        if (val == null) val = false;
                        values.Append($"{paramName},");
                        parameterList.Add(paramName, Convert.ToBoolean(val));
                        parameterCount++;
                    }
                    else if (tableColumn.DataType == DataType.DateTime)
                    {
                        if (val == null) val = DateTime.Now;
                        values.Append($"{GetDateTimeSqlString(Convert.ToDateTime(val))},");
                    }
                    else
                    {
                        var paramName = $"@{tableColumn.AttributeName}_{index}";
                        values.Append($"{paramName},");
                        parameterList.Add(paramName, Convert.ToString(val));
                        parameterCount++;
                    }
                }

                if (values.Length > 0)
                {
                    values.Length -= 1;
                    valuesList.Add(values.ToString());

                    if (parameterCount > 1000)
                    {
                        await InsertRowsAsync(database, tableName, columnNames.ToString(), valuesList, parameterList);
                        valuesList.Clear();
                        parameterList = new DynamicParameters();
                        parameterCount = 0;
                    }
                }
            }

            if (valuesList.Count > 0 && parameterCount > 0)
            {
                await InsertRowsAsync(database, tableName, columnNames.ToString(), valuesList, parameterList);
            }
        }

        private static Dictionary<string, object> JsonGetDictionaryIgnoreCase(JObject json)
        {
            return new Dictionary<string, object>(json.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
        }

        private static string GetDateTimeSqlString(DateTime dateTime)
        {
            return $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
        }

        private static async Task InsertRowsAsync(IDatabase database, string tableName, string columnNames, List<string> valuesList, DynamicParameters parameterList)
        {
            if (database.DatabaseType == DatabaseType.SqlServer)
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                var sqlString = sqlStringBuilder.ToString();

                //var isIdentityColumn = !Utilities.EqualsIgnoreCase(tableName, "siteserver_Site");
                //if (isIdentityColumn)
                //{
                    sqlString = $@"
SET IDENTITY_INSERT {tableName} ON
{sqlString}
SET IDENTITY_INSERT {tableName} OFF
";
                //}

                using var connection = database.GetConnection();
                await connection.ExecuteAsync(sqlString, parameterList);
            }
            else
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                using var connection = database.GetConnection();
                await connection.ExecuteAsync(sqlStringBuilder.ToString(), parameterList);
            }
        }
    }
}
