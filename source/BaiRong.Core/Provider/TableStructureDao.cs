using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using MySql.Data.MySqlClient;

namespace BaiRong.Core.Provider
{
    public class TableStructureDao : DataProviderBase
    {
        public List<string> GetDatabaseNameList(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var list = new List<string>();

            if (WebConfigUtils.IsMySql)
            {
                var connection = new MySqlConnection(connectionString);
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
            else
            {
                var connection = new SqlConnection(connectionString);
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

            return list;
        }

        public IDictionary GetTablesAndViewsDictionary(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            string sqlString =
                $"select name, id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 Order By Name";

            var sortedlist = new SortedList();

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    sortedlist.Add(GetString(rdr, 0), GetInt(rdr, 1));
                }
                rdr.Close();
            }
            return sortedlist;
        }

        public bool IsTableExists(string tableName)
        {
            var exists = false;
            string sqlString;
            if (WebConfigUtils.IsMySql)
            {
                sqlString = $@"show tables like ""{tableName}""";
            }
            else
            {
                sqlString =
                $"select * from dbo.sysobjects where id = object_id(N'{tableName}') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public string GetTableId(string connectionString, string databaseName, string tableName)
        {
            if (WebConfigUtils.IsMySql) return tableName;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var tableId = SqlUtils.Cache_GetTableIDCache(databaseName, tableName);

            if (string.IsNullOrEmpty(tableId))
            {
                string sqlString =
                    $"select id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 and name='{tableName}'";

                using (var rdr = ExecuteReader(connectionString, sqlString))
                {
                    if (rdr.Read())
                    {
                        tableId = GetString(rdr, 0);
                        SqlUtils.Cache_CacheTableID(databaseName, tableName, tableId);
                    }
                    rdr.Close();
                }
            }

            return tableId;
        }

        public string GetTableName(string databaseName, string tableId)
        {
            if (WebConfigUtils.IsMySql) return tableId;

            var tableName = string.Empty;
            string cmd =
                $"select O.name from [{databaseName}].dbo.sysobjects O, [{databaseName}].dbo.sysusers U where O.id={tableId} and U.uid=O.uid";

            using (var rdr = ExecuteReader(cmd))
            {
                if (rdr.Read())
                {
                    tableName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return tableName;
        }

        public string GetTableName(string connectionString, string databaseName, string tableId)
        {
            if (WebConfigUtils.IsMySql) return tableId;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var tableName = string.Empty;
            string sqlString =
                $"select O.name from [{databaseName}].dbo.sysobjects O, [{databaseName}].dbo.sysusers U where O.id={tableId} and U.uid=O.uid";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    tableName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return tableName;
        }

        public string GetDefaultConstraintName(string tableName, string columnName)
        {
            if (!WebConfigUtils.IsMySql) return string.Empty;

            var defaultConstraintName = string.Empty;
            string sqlString =
                $"select b.name from syscolumns a,sysobjects b where a.id=object_id('{tableName}') and b.id=a.cdefault and a.name='{columnName}' and b.name like 'DF%'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    defaultConstraintName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return defaultConstraintName;
        }

        public List<string> GetColumnNameList(string tableName)
        {
            return GetColumnNameList(tableName, false);
        }

        public List<string> GetColumnNameList(string tableName, bool isLower)
        {
            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.ConnectionString);
            var tableId = GetTableId(WebConfigUtils.ConnectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(databaseName, tableName, tableId);

            var columnNameList = new List<string>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                columnNameList.Add(isLower ? tableColumnInfo.ColumnName.ToLower() : tableColumnInfo.ColumnName);
            }

            return columnNameList;
        }

        public List<string> GetColumnNameList(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(connectionString);
            var tableId = GetTableId(connectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(connectionString, databaseName, tableName, tableId);

            var columnNameList = new List<string>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                columnNameList.Add(tableColumnInfo.ColumnName);
            }

            return columnNameList;
        }

        public List<TableColumnInfo> GetTableColumnInfoList(string databaseName, string tableName, string tableId)
        {
            return GetTableColumnInfoList(WebConfigUtils.ConnectionString, databaseName, tableName, tableId);
        }

        public List<TableColumnInfo> GetTableColumnInfoList(string connectionString, string databaseName, string tableName, string tableId)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var cacheList = SqlUtils.Cache_GetTableColumnInfoListCache(connectionString, databaseName, tableId);
            if (cacheList != null && cacheList.Count > 0)
            {
                return cacheList;
            }

            var list = new List<TableColumnInfo>();
            var isIdentityExist = false;

            if (WebConfigUtils.IsMySql)
            {
                string sqlString = $"select COLUMN_NAME, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, COLUMN_KEY from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";
                using (var rdr = ExecuteReader(connectionString, sqlString))
                {
                    while (rdr.Read())
                    {
                        var columnName = Convert.ToString(rdr.GetValue(0));
                        var isNullable = Convert.ToString(rdr.GetValue(1)) == "YES";
                        var dataType = EDataTypeUtils.FromMySql(Convert.ToString(rdr.GetValue(2)));
                        var length = rdr.IsDBNull(3) || dataType == EDataType.NText || dataType == EDataType.Text ? 0 : Convert.ToInt32(rdr.GetValue(3));
                        var precision = rdr.IsDBNull(4) ? 0 : Convert.ToInt32(rdr.GetValue(4));
                        var scale = rdr.IsDBNull(5) ? 0 : Convert.ToInt32(rdr.GetValue(5));
                        var isPrimaryKey = Convert.ToString(rdr.GetValue(6)) == "PRI";
                        
                        var isIdentity = isPrimaryKey && StringUtils.EqualsIgnoreCase(columnName, "ID");

                        var info = new TableColumnInfo(databaseName, tableId, columnName, dataType, length, precision, scale, isPrimaryKey, isNullable, isIdentity);
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }
            else
            {
                string sqlString =
                $"select C.name, T.name, C.length, C.xprec, C.xscale, C.colstat, C.isnullable, case when C.autoval is null then 0 else 1 end, SC.text, (select CForgin.name from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select O.name from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select Sr.rkeyid from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1) from [{databaseName}].dbo.systypes T, [{databaseName}].dbo.syscolumns C left join [{databaseName}].dbo.syscomments SC on C.cdefault=SC.id where C.id={tableId} and C.xtype=T.xusertype order by C.colid";

                using (var rdr = ExecuteReader(connectionString, sqlString))
                {
                    while (rdr.Read())
                    {
                        var columnName = Convert.ToString(rdr.GetValue(0));
                        if (columnName == "msrepl_tran_version")//sqlserver 发布订阅字段，忽略
                        {
                            continue;
                        }
                        var dataType = EDataTypeUtils.FromSqlServer(Convert.ToString(rdr.GetValue(1)));
                        var length = GetDataLength(dataType, Convert.ToInt32(rdr.GetValue(2)));
                        var precision = Convert.ToInt32(rdr.GetValue(3));
                        var scale = Convert.ToInt32(rdr.GetValue(4));
                        var isPrimaryKeyInt = Convert.ToInt32(rdr.GetValue(5));
                        var isNullableInt = Convert.ToInt32(rdr.GetValue(6));
                        var isIdentityInt = Convert.ToInt32(rdr.GetValue(7));

                        var isPrimaryKey = isPrimaryKeyInt == 1;
                        var isNullable = isNullableInt == 1;
                        var isIdentity = isIdentityInt == 1 || StringUtils.EqualsIgnoreCase(columnName, "ID");
                        //sqlserver 2005 返回isIdentity结果不正确,so 在此假设所有ID字段为Idenity字段
                        if (isIdentity)
                        {
                            isIdentityExist = true;
                        }

                        var info = new TableColumnInfo(databaseName, tableId, columnName, dataType, length, precision, scale, isPrimaryKey, isNullable, isIdentity);
                        list.Add(info);
                    }
                    rdr.Close();
                }

                if (!isIdentityExist)
                {
                    var sqlIdentity = "select name from syscolumns where id = object_id(N'" + tableName + "') and COLUMNPROPERTY(id, name,'IsIdentity')= 1";
                    var clName = "";
                    using (var rdr = ExecuteReader(sqlIdentity))
                    {
                        if (rdr.Read())
                        {
                            clName = GetString(rdr, 0);
                        }
                        rdr.Close();
                    }

                    foreach (var info in list)
                    {
                        if (clName == info.ColumnName)
                        {
                            info.IsIdentity = true;
                        }
                    }
                }
            }

            SqlUtils.Cache_CacheTableColumnInfoList(connectionString, databaseName, tableId, list);

            return list;
        }

        //lengthFromDb:数据库元数据查询获取的长度
        protected int GetDataLength(EDataType dataType, int lengthFromDb)
        {
            if (dataType == EDataType.NChar || dataType == EDataType.NVarChar)
            {
                return Convert.ToInt32(lengthFromDb / 2);
            }

            return lengthFromDb;
        }

        public bool IsColumnEquals(TableMetadataInfo metadataInfo, TableColumnInfo columnInfo)
        {
            if (!StringUtils.EqualsIgnoreCase(metadataInfo.AttributeName, columnInfo.ColumnName)) return false;
            if (metadataInfo.DataType != columnInfo.DataType) return false;
            if (metadataInfo.DataLength != columnInfo.Length) return false;
            return true;
        }

        public string GetInsertSqlString(NameValueCollection attributes, string tableName, out IDataParameter[] parms)
        {
            return GetInsertSqlString(attributes, ConnectionString, tableName, out parms);
        }

        public string GetInsertSqlString(NameValueCollection attributes, string connectionString, string tableName, out IDataParameter[] parms)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }
            //by 20151030 sofuny 获取自动增长列

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(connectionString);
            var tableId = GetTableId(connectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(connectionString, databaseName, tableName, tableId);

            var columnNameList = new List<string>();

            var parameterList = new List<IDataParameter>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                if (!tableColumnInfo.IsIdentity)
                {
                    if (attributes[tableColumnInfo.ColumnName] == null)
                    {
                        if (!tableColumnInfo.IsNullable)
                        {
                            columnNameList.Add(tableColumnInfo.ColumnName);
                            var valueStr = string.Empty;

                            if (tableColumnInfo.DataType == EDataType.DateTime)
                            {
                                parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToDateTime(valueStr)));
                            }
                            else if (tableColumnInfo.DataType == EDataType.Integer)
                            {
                                parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToIntWithNagetive(valueStr)));
                            }
                            else
                            {
                                parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                            }
                        }
                    }
                    else
                    {
                        columnNameList.Add(tableColumnInfo.ColumnName);
                        var valueStr = attributes[tableColumnInfo.ColumnName];

                        if (tableColumnInfo.DataType == EDataType.DateTime)
                        {
                            parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToDateTime(valueStr)));
                        }
                        else if (tableColumnInfo.DataType == EDataType.Integer)
                        {
                            parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToIntWithNagetive(valueStr)));
                        }
                        else
                        {
                            parameterList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                        }
                    }
                }
            }

            parms = parameterList.ToArray();

            string returnSqlString =
                $"INSERT INTO {tableName} ({TranslateUtils.ObjectCollectionToString(columnNameList, " ,", "[", "]")}) VALUES ({TranslateUtils.ObjectCollectionToString(columnNameList, " ,", "@")})";

            return returnSqlString;
        }

        public string GetUpdateSqlString(NameValueCollection attributes, string tableName, out IDataParameter[] parms)
        {
            return GetUpdateSqlString(attributes, ConnectionString, tableName, out parms);
        }

        public string GetUpdateSqlString(NameValueCollection attributes, string connectionString, string tableName, out IDataParameter[] parms)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            parms = null;
            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(connectionString);
            var tableId = GetTableId(connectionString, databaseName, tableName);
            var allTableColumnInfoList = GetTableColumnInfoList(connectionString, databaseName, tableName, tableId);

            var setList = new List<string>();
            var whereList = new List<string>();

            var parmsList = new List<IDataParameter>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                if (attributes[tableColumnInfo.ColumnName] != null)
                {
                    if (!tableColumnInfo.IsPrimaryKey && tableColumnInfo.ColumnName != "ID")
                    {
                        var valueStr = attributes[tableColumnInfo.ColumnName];
                        var sqlValue = SqlUtils.Parse(tableColumnInfo.DataType, valueStr, tableColumnInfo.Length);
                        if (!string.IsNullOrEmpty(sqlValue))
                        {
                            setList.Add($"{tableColumnInfo.ColumnName} = {"@" + tableColumnInfo.ColumnName}");

                            if (tableColumnInfo.DataType == EDataType.DateTime)
                            {
                                parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToDateTime(valueStr)));
                            }
                            else if (tableColumnInfo.DataType == EDataType.Integer)
                            {
                                parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, TranslateUtils.ToInt(valueStr)));
                            }
                            else
                            {
                                parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                            }
                        }

                    }
                    else
                    {
                        var valueStr = attributes[tableColumnInfo.ColumnName];
                        whereList.Add($"{tableColumnInfo.ColumnName} = {"@" + tableColumnInfo.ColumnName}");
                        parmsList.Add(GetParameter("@" + tableColumnInfo.ColumnName, tableColumnInfo.DataType, valueStr));
                    }
                }
            }

            if (whereList.Count == 0 && !string.IsNullOrEmpty(attributes["ID"]))
            {
                whereList.Add("ID = @ID");
            }

            if (whereList.Count == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            if (setList.Count == 0)
            {
                throw new SyntaxErrorException();
            }

            parms = parmsList.ToArray();

            string returnSqlString =
                $"UPDATE {tableName} SET {TranslateUtils.ObjectCollectionToString(setList, " ,")} WHERE {TranslateUtils.ObjectCollectionToString(whereList, " AND ")}";

            return returnSqlString;
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, null);
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
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

            //if (totalNum > 0)
            //{
            //    sqlString = $"SELECT TOP {totalNum} {columns} FROM [{tableName}] {whereString} {orderByString}";
            //}
            //else
            //{
            //    sqlString = $"SELECT {columns} FROM {tableName} {whereString} {orderByString}";
            //}
            return SqlUtils.GetTopSqlString(tableName, columns, whereString + " " + orderByString, totalNum);
        }

        public string GetSelectSqlString(string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, startNum, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (startNum <= 1)
            {
                return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
            }

            string countSqlString = $"SELECT Count(*) FROM {tableName} {whereString}";
            var count = BaiRongDataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
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
                    return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            if (WebConfigUtils.IsMySql)
            {
                return $@"
SELECT {columns} FROM (
    SELECT {columns} FROM (
        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}
";
            }
            else
            {
                return $@"
SELECT {columns}
FROM (SELECT TOP {totalNum} {columns}
        FROM (SELECT TOP {topNum} {columns}
                FROM {tableName} {whereString} {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
        }

        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int totalNum, string orderByString)
        {
            if (totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            string sqlString;
            if (totalNum > 0)
            {
                //TODO: 当queryString包含top 2语句时排序有问题
                if (WebConfigUtils.IsMySql)
                {
                    sqlString = $"SELECT * FROM ({queryString}) AS tmp {orderByString} LIMIT {totalNum}";
                }
                else
                {
                    sqlString = $"SELECT TOP {totalNum} * FROM ({queryString}) tmp {orderByString}";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(orderByString))
                {
                    sqlString = queryString;
                }
                else
                {
                    sqlString = $"SELECT * FROM ({queryString}) tmp {orderByString}";
                }
            }
            return sqlString;
        }


        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
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
            var count = BaiRongDataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
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

            if (WebConfigUtils.IsMySql)
            {
                return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else
            {
                return $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
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
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(orderByString))
            {
                retval = orderByString.Replace(" DESC", " DESC_OPPOSITE").Replace(" ASC", " DESC").Replace(" DESC_OPPOSITE", " ASC");
            }
            return retval;
        }
    }
}
