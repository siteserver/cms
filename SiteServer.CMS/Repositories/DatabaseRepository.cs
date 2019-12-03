using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Datory;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public class DatabaseRepository : DataProviderBase
    {
        public async Task DeleteDbLogAsync()
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                using var connection = WebConfigUtils.Database.GetConnection();
                await connection.ExecuteAsync("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var databaseName = await WebConfigUtils.Database.GetDatabaseNamesAsync();

                using var connection = WebConfigUtils.Database.GetConnection();
                var versions = await connection.QueryFirstAsync<string>("SELECT SERVERPROPERTY('productversion')");

                var version = 8;
                var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0)
                {
                    version = TranslateUtils.ToInt(arr[0], 8);
                }
                if (version < 10)
                {
                    await connection.ExecuteAsync($"BACKUP LOG [{databaseName}] WITH NO_LOG");
                }
                else
                {
                    await connection.ExecuteAsync($@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ");
                }
            }
        }

        public int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            var count = 0;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        count = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public int GetIntResult(string sqlString)
        {
            return GetIntResult(WebConfigUtils.ConnectionString, sqlString);
        }

        public string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            var retVal = string.Empty;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        retVal = GetString(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return retVal;
        }

        public string GetString(string sqlString)
        {
            var value = string.Empty;
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    value = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return value;
        }

        public IDataReader GetDataReader(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            return string.IsNullOrEmpty(sqlString) ? null : ExecuteReader(connectionString, sqlString);
        }

        public IDataReader GetDataSource(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = ExecuteReader(sqlString);
            return enumerable;
        }

        public DataTable GetDataTable(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(connectionString, sqlString);

            if (dataset == null || dataset.Tables.Count == 0) return null;

            return dataset.Tables[0];
        }

        public DataSet GetDataSet(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            return ExecuteDataset(connectionString, sqlString);
        }

        public int GetPageTotalCount(string sqlString)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            // Add new ORDER BY info if SortKeyField is specified
            //if (!string.IsNullOrEmpty(sortField) && addCustomSortInfo)
            //    SelectCommand += " ORDER BY " + SortField;

            var cmdText = WebConfigUtils.DatabaseType == DatabaseType.Oracle
                ? $"SELECT COUNT(*) FROM ({sqlString})"
                : $"SELECT COUNT(*) FROM ({sqlString}) AS T0";
            return GetIntResult(cmdText);
        }

        public string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
        {
            var retVal = string.Empty;

            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = totalCount / itemsPerPage;
            var remainder = totalCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            orderString = orderString.ToUpper();
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {recsToRetrieve} {orderStringReverse}
) {orderString}";
            }

            //            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            //            {
            //                return $@"
            //SELECT * FROM (
            //    SELECT * FROM (
            //        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
            //    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
            //) AS t2 {orderString}";
            //            }
            //            else
            //            {
            //                return $@"
            //SELECT * FROM (
            //    SELECT TOP {recsToRetrieve} * FROM (
            //        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
            //    ) AS t1 {orderStringReverse}
            //) AS t2 {orderString}";
            //            }

            return retVal;
        }

        public bool ConnectToServer(DatabaseType databaseType, string connectionStringWithoutDatabaseName, out List<string> databaseNameList, out string errorMessage)
        {
            errorMessage = string.Empty;
            databaseNameList = new List<string>();
            try
            {
                databaseNameList = GetDatabaseNameList(databaseType, connectionStringWithoutDatabaseName);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return false;
        }

        public string GetCreateTableSqlString(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            var primaryKeyColumns = new List<TableColumn>();
            TableColumn identityColumn = null;
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName)) continue;

                if (tableColumn.IsIdentity)
                {
                    identityColumn = tableColumn;
                }

                if (tableColumn.IsPrimaryKey)
                {
                    primaryKeyColumns.Add(tableColumn);
                }

                var columnSql = SqlUtils.GetColumnSqlString(tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

            if (identityColumn != null)
            {
                var primarykeySql = SqlUtils.GetPrimaryKeySqlString(tableName, identityColumn.AttributeName);
                if (!string.IsNullOrEmpty(primarykeySql))
                {
                    sqlBuilder.Append(primarykeySql).Append(",");
                }
            }
            else if (primaryKeyColumns.Count > 0)
            {
                foreach (var tableColumn in primaryKeyColumns)
                {
                    var primarykeySql = SqlUtils.GetPrimaryKeySqlString(tableName, tableColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primarykeySql))
                    {
                        sqlBuilder.Append(primarykeySql).Append(",");
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");

            return sqlBuilder.ToString();
        }

        public async Task<(bool Success, Exception Ex, string SqlString)> CreateTableAsync(string tableName, List<TableColumn> tableColumns)
        {
            var sqlString = GetCreateTableSqlString(tableName, tableColumns);

            try
            {
                ExecuteNonQuery(sqlString);
                TableColumnManager.ClearCache();
                return (true, null, sqlString);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex, tableName);
                return (false, ex, sqlString);
            }
        }

        public async Task AlterSystemTableAsync(string tableName, List<TableColumn> tableColumns, List<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (StringUtils.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    var databaseColumn = TableColumnManager.GetTableColumnInfo(tableName, tableColumn.AttributeName);
                    if (databaseColumn != null && !tableColumn.IsIdentity)
                    {
                        if (tableColumn.DataType != databaseColumn.DataType ||
                            tableColumn.DataType == databaseColumn.DataType && tableColumn.DataLength > databaseColumn.DataLength)
                        {
                            list.Add(SqlUtils.GetModifyColumnsSqlString(tableName, tableColumn.AttributeName, SqlUtils.GetColumnTypeString(tableColumn)));
                        }
                    }
                }
                else
                {
                    list.Add(SqlUtils.GetAddColumnsSqlString(tableName, SqlUtils.GetColumnSqlString(tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (StringUtils.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(SqlUtils.GetDropColumnsSqlString(tableName, columnName));
                    }
                }
            }

            if (list.Count > 0)
            {
                using var connection = WebConfigUtils.Database.GetConnection();
                foreach (var sqlString in list)
                {
                    try
                    {
                        await connection.ExecuteAsync(sqlString);
                    }
                    catch (Exception ex)
                    {
                        await LogUtils.AddErrorLogAsync(ex, sqlString);
                    }
                }

                TableColumnManager.ClearCache();
            }
        }

        public bool DropTable(string tableName)
        {
            try
            {
                ExecuteNonQuery($"DROP TABLE {tableName}");
                TableColumnManager.ClearCache();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<string> GetDatabaseNameList(DatabaseType databaseType, string connectionStringWithoutDatabaseName)
        {
            if (string.IsNullOrEmpty(connectionStringWithoutDatabaseName))
            {
                connectionStringWithoutDatabaseName = WebConfigUtils.ConnectionString;
            }

            var list = new List<string>();

            if (databaseType == DatabaseType.MySql)
            {
                var connection = new MySqlConnection(connectionStringWithoutDatabaseName);
                var command = new MySqlCommand("show databases", connection);

                connection.Open();

                var rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    var dbName = rdr.GetString(0);
                    if (dbName == null) continue;
                    if (dbName != "information_schema" &&
                        dbName != "mysql" &&
                        dbName != "performance_schema" &&
                        dbName != "sakila" &&
                        dbName != "sys" &&
                        dbName != "world")
                    {
                        list.Add(dbName);
                    }
                }

                connection.Close();
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                var connection = new SqlConnection(connectionStringWithoutDatabaseName);
                var command = new SqlCommand("select name from master..sysdatabases order by name asc", connection);

                connection.Open();

                connection.ChangeDatabase("master");

                var dr = command.ExecuteReader();

                while (dr.Read())
                {
                    var dbName = dr["name"] as string;
                    if (dbName == null) continue;
                    if (dbName != "master" &&
                        dbName != "msdb" &&
                        dbName != "tempdb" &&
                        dbName != "model")
                    {
                        list.Add(dbName);
                    }
                }

                connection.Close();
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                var connection = new NpgsqlConnection(connectionStringWithoutDatabaseName);
                var command =
                    new NpgsqlCommand(
                        "select datname from pg_database where datistemplate = false order by datname asc",
                        connection);

                connection.Open();

                var dr = command.ExecuteReader();

                while (dr.Read())
                {
                    var dbName = dr["datname"] as string;
                    if (dbName == null) continue;

                    list.Add(dbName);
                }

                connection.Close();
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                var connection = new OracleConnection(connectionStringWithoutDatabaseName);
                //var command = new OracleCommand("select name from v$database", connection);

                connection.Open();

                //var rdr = command.ExecuteReader();

                //while (rdr.Read())
                //{
                //    var dbName = rdr.GetString(0);
                //    if (dbName == null) continue;
                //    list.Add(dbName);
                //}

                connection.Close();
            }

            return list;
        }

        public List<TableColumn> GetTableColumnInfoList(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, connectionString);

            List<TableColumn> list = null;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                list = GetMySqlColumns(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                list = GetSqlServerColumns(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                list = GetPostgreSqlColumns(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                list = GetOracleColumns(connectionString, tableName);
            }

            return list;
        }

        private List<TableColumn> GetOracleColumns(string connectionString, string tableName)
        {
            var owner = WebConfigUtils.GetConnectionStringUserId(connectionString).ToUpper();
            tableName = tableName.ToUpper();

            var list = new List<TableColumn>();
            var sqlString =
                $"SELECT COLUMN_NAME, DATA_TYPE, DATA_PRECISION, DATA_SCALE, CHAR_LENGTH, DATA_DEFAULT FROM all_tab_cols WHERE OWNER = '{owner}' and table_name = '{tableName}' and user_generated = 'YES' ORDER BY COLUMN_ID";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var percision = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);
                    var scale = rdr.IsDBNull(3) ? 0 : rdr.GetInt32(3);
                    var charLength = rdr.IsDBNull(4) ? 0 : rdr.GetInt32(4);
                    var dataDefault = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5);
                    if (dataType == DataType.Integer)
                    {
                        if (scale == 2)
                        {
                            dataType = DataType.Decimal;
                        }
                        else if (percision == 1)
                        {
                            dataType = DataType.Boolean;
                        }
                    }
                    var isIdentity = dataDefault.Contains(".nextval");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = charLength,
                        IsPrimaryKey = false,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            sqlString =
                $@"select distinct cu.column_name from all_cons_columns cu inner join all_constraints au 
on cu.constraint_name = au.constraint_name
and au.constraint_type = 'P' and cu.OWNER = '{owner}' and cu.table_name = '{tableName}'";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);

                    foreach (var tableColumnInfo in list)
                    {
                        if (columnName == tableColumnInfo.AttributeName)
                        {
                            tableColumnInfo.IsPrimaryKey = true;
                            break;
                        }
                    }
                }
                rdr.Close();
            }

            return list;
        }

        private List<TableColumn> GetPostgreSqlColumns(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumn>();
            var sqlString =
                $"SELECT COLUMN_NAME, UDT_NAME, CHARACTER_MAXIMUM_LENGTH, COLUMN_DEFAULT FROM information_schema.columns WHERE table_catalog = '{databaseName}' AND table_name = '{tableName.ToLower()}' ORDER BY ordinal_position";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);
                    var columnDefault = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3);

                    var isIdentity = columnDefault.StartsWith("nextval(");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = length,
                        IsPrimaryKey = false,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            sqlString =
                $"select column_name, constraint_name from information_schema.key_column_usage where table_catalog = '{databaseName}' and table_name = '{tableName.ToLower()}';";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var constraintName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);

                    var isPrimary = constraintName.StartsWith("pk");

                    if (isPrimary)
                    {
                        foreach (var tableColumnInfo in list)
                        {
                            if (columnName == tableColumnInfo.AttributeName)
                            {
                                tableColumnInfo.IsPrimaryKey = true;
                                break;
                            }
                        }
                    }
                }
                rdr.Close();
            }

            return list;
        }

        private List<TableColumn> GetSqlServerColumns(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumn>();

            var isIdentityExist = false;
            var tableId = string.Empty;
            var sqlStringTableId =
                $"select id from [{databaseName}]..sysobjects where type = 'U' and category<>2 and name='{tableName}'";

            using (var rdr = ExecuteReader(connectionString, sqlStringTableId))
            {
                if (rdr.Read())
                {
                    tableId = GetString(rdr, 0);
                }
                rdr.Close();
            }

            var sqlString =
                $"select C.name, T.name, C.length, C.colstat, case when C.autoval is null then 0 else 1 end, SC.text, (select CForgin.name from [{databaseName}]..sysreferences Sr,[{databaseName}]..sysobjects O,[{databaseName}]..syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select O.name from [{databaseName}]..sysreferences Sr,[{databaseName}]..sysobjects O,[{databaseName}]..syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select Sr.rkeyid from [{databaseName}]..sysreferences Sr,[{databaseName}]..sysobjects O,[{databaseName}]..syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1) from [{databaseName}]..systypes T, [{databaseName}]..syscolumns C left join [{databaseName}]..syscomments SC on C.cdefault=SC.id where C.id={tableId} and C.xtype=T.xusertype order by C.colid";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    if (columnName == "msrepl_tran_version")
                    {
                        continue;
                    }

                    var dataTypeName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
                    var dataType = SqlUtils.ToDataType(dataTypeName);
                    var length = Convert.ToInt32(rdr.GetValue(2));
                    if (dataType == DataType.VarChar && dataTypeName == "nvarchar")
                    {
                        length = Convert.ToInt32(length / 2);
                    }
                    var isPrimaryKeyInt = Convert.ToInt32(rdr.GetValue(3));
                    var isIdentityInt = Convert.ToInt32(rdr.GetValue(4));

                    var isPrimaryKey = isPrimaryKeyInt == 1;
                    //var isIdentity = isIdentityInt == 1 || StringUtils.EqualsIgnoreCase(columnName, "Id");
                    var isIdentity = isIdentityInt == 1;
                    if (isIdentity)
                    {
                        isIdentityExist = true;
                    }

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = length,
                        IsPrimaryKey = isPrimaryKey,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            if (!isIdentityExist)
            {
                var sqlIdentity = "select name from syscolumns where id = object_id(N'" + tableName +
                                  "') and COLUMNPROPERTY(id, name,'IsIdentity')= 1";
                var clName = "";
                using (var rdr = ExecuteReader(sqlIdentity))
                {
                    if (rdr.Read())
                    {
                        clName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    }
                    rdr.Close();
                }

                foreach (var info in list)
                {
                    if (clName == info.AttributeName)
                    {
                        info.IsIdentity = true;
                    }
                }
            }

            return list;
        }

        private List<TableColumn> GetMySqlColumns(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumn>();

            string sqlString =
                $"select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_KEY, EXTRA from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = rdr.IsDBNull(2) || dataType == DataType.Text ? 0 : Convert.ToInt32(rdr.GetValue(2));
                    var isPrimaryKey = Convert.ToString(rdr.GetValue(3)) == "PRI";
                    var isIdentity = Convert.ToString(rdr.GetValue(4)) == "auto_increment";

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = length,
                        IsPrimaryKey = isPrimaryKey,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, null);
        }

        public string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(WebConfigUtils.ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        private string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        private string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = StringUtils.ReplaceStartsWith(whereString.Trim(), "AND", string.Empty);
                if (!StringUtils.StartsWithIgnoreCase(whereString, "WHERE "))
                {
                    whereString = "WHERE " + whereString;
                }
            }

            if (!string.IsNullOrEmpty(joinString))
            {
                whereString = joinString + " " + whereString;
            }

            return SqlUtils.ToTopSqlString(tableName, columns, whereString, orderByString, totalNum);
        }

        private string GetSelectSqlStringByQueryString(string connectionString, string queryString, int totalNum, string orderByString)
        {
            if (totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            string sqlString;
            if (totalNum > 0)
            {
                sqlString = SqlUtils.ToTopSqlString(queryString, orderByString, totalNum);

                //sqlString = WebConfigUtils.DatabaseType == DatabaseType.MySql ? $"SELECT * FROM ({queryString}) AS tmp {orderByString} LIMIT {totalNum}" : $"SELECT TOP {totalNum} * FROM ({queryString}) tmp {orderByString}";
            }
            else
            {
                sqlString = string.IsNullOrEmpty(orderByString) ? queryString : $"SELECT * FROM ({queryString}) {orderByString}";
            }
            return sqlString;
        }


        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (startNum == 1 && totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            //queryString = queryString.Trim().ToUpper();
            if (queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal) != -1)
            {
                if (string.IsNullOrEmpty(orderByString))
                {
                    orderByString = queryString.Substring(queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal) + 1);
                }
                queryString = queryString.Substring(0, queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal));
            }
            orderByString = ParseOrderByString(orderByString);

            if (startNum <= 1)
            {
                return GetSelectSqlStringByQueryString(connectionString, queryString, totalNum, orderByString);
            }

            string countSqlString = $"SELECT Count(*) FROM ({queryString}) tmp";
            var count = DataProvider.DatabaseRepository.GetIntResult(connectionString, countSqlString);
            if (totalNum == 0)
            {
                totalNum = count;
            }

            if (startNum > count) return string.Empty;

            var topNum = startNum + totalNum - 1;

            if (count < topNum)
            {
                totalNum = count - startNum + 1;
                if (totalNum < 1)
                {
                    return GetSelectSqlStringByQueryString(connectionString, queryString, totalNum, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }

            return retVal;
        }

        private static string ParseOrderByString(string orderByString)
        {
            if (string.IsNullOrEmpty(orderByString)) return orderByString;

            orderByString = orderByString.ToUpper().Trim();
            if (!orderByString.StartsWith("ORDER BY"))
            {
                orderByString = "ORDER BY " + orderByString;
            }
            if (!orderByString.EndsWith("DESC") && !orderByString.EndsWith("ASC"))
            {
                orderByString = orderByString + " ASC";
            }
            return orderByString;
        }

        private string GetOrderByStringOpposite(string orderByString)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(orderByString))
            {
                retVal = orderByString.Replace(" DESC", " DESC_OPPOSITE").Replace(" ASC", " DESC").Replace(" DESC_OPPOSITE", " ASC");
            }
            return retVal;
        }

        public List<string> GetTableNameList()
        {
            var list = new List<string>();

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                var sqlString = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{databaseName}' ORDER BY table_name";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var sqlString =
                    $"SELECT name FROM [{databaseName}]..sysobjects WHERE type = 'U' AND category<>2 ORDER BY Name";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                var sqlString =
                    $"SELECT table_name FROM information_schema.tables WHERE table_catalog = '{databaseName}' AND table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                const string sqlString = "select TABLE_NAME from user_tables";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public int GetCount(string tableName)
        {
            int count;

            using (var conn = WebConfigUtils.Database.GetConnection())
            {
                count = conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM {SqlUtils.GetQuotedIdentifier(tableName)}");
            }
            return count;

            //return GetIntResult();
        }

        public IEnumerable<dynamic> GetObjects(string tableName)
        {
            IEnumerable<dynamic> objects;
            var sqlString = $"select * from {tableName}";

            using (var connection = WebConfigUtils.Database.GetConnection())
            {
                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit)
        {
            IEnumerable<dynamic> objects;
            var sqlString = GetPageSqlString(tableName, "*", string.Empty, $"ORDER BY {identityColumnName} ASC", offset, limit);

            using (var connection = WebConfigUtils.Database.GetConnection())
            {
                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        private decimal? _sqlServerVersion;

        private decimal SqlServerVersion
        {
            get
            {
                if (WebConfigUtils.DatabaseType != DatabaseType.SqlServer)
                {
                    return 0;
                }

                if (_sqlServerVersion == null)
                {
                    try
                    {
                        _sqlServerVersion =
                            TranslateUtils.ToDecimal(
                                GetString("select left(cast(serverproperty('productversion') as varchar), 4)"));
                    }
                    catch
                    {
                        _sqlServerVersion = 0;
                    }
                }

                return _sqlServerVersion.Value;
            }
        }

        private bool IsSqlServer2012 => SqlServerVersion >= 11;

        public int GetPageTotalCount(string tableName, string whereSqlString)
        {
            return GetIntResult($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}");
        }

        public int GetPageTotalCount(string tableName, string whereSqlString, Dictionary<string, object> parameters)
        {
            int totalCount;

            using (var connection = WebConfigUtils.Database.GetConnection())
            {
                totalCount = connection.QueryFirstOrDefault<int>($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}", parameters);
            }

            return totalCount;
        }

        public string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString, int offset, int limit)
        {
            var retVal = string.Empty;

            if (string.IsNullOrEmpty(orderSqlString))
            {
                orderSqlString = "ORDER BY Id DESC";
            }

            if (offset == 0 && limit == 0)
            {
                return $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                if (limit == 0)
                {
                    limit = int.MaxValue;
                }
                retVal = $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && IsSqlServer2012)
            {
                retVal = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && !IsSqlServer2012)
            {
                if (offset == 0)
                {
                    retVal = $"SELECT TOP {limit} {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
                }
                else
                {
                    var rowWhere = limit == 0
                        ? $@"WHERE [row_num] > {offset}"
                        : $@"WHERE [row_num] BETWEEN {offset + 1} AND {offset + limit}";

                    retVal = $@"SELECT * FROM (
    SELECT {columnNames}, ROW_NUMBER() OVER ({orderSqlString}) AS [row_num] FROM [{tableName}] {whereSqlString}
) as T {rowWhere}";
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = limit == 0
                    ? $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }

            return retVal;
        }
    }
}

