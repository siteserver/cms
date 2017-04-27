using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using MySql.Data.MySqlClient;

namespace BaiRong.Core.Provider
{
    public class DatabaseDao : DataProviderBase
    {
        public virtual void DeleteDbLog()
        {
            if (WebConfigUtils.IsMySql)
            {
                ExecuteSql("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
                return;
            }

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.ConnectionString);
            //检测数据库版本
            const string sqlCheck = "SELECT SERVERPROPERTY('productversion')";
            var versions = ExecuteScalar(sqlCheck).ToString();
            //MM.nn.bbbb.rr
            //8 -- 2000
            //9 -- 2005
            //10 -- 2008
            var version = 8;
            var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                version = TranslateUtils.ToInt(arr[0], 8);
            }
            if (version < 10)
            {
                //2000,2005
                string sql = $"BACKUP LOG [{databaseName}] WITH NO_LOG";
                ExecuteNonQuery(sql);
            }
            else
            {
                //2008+
                string sql =
                    $@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ";
                ExecuteNonQuery(sql);
            }
        }

        public void ExecuteSql(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return;

            using (var conn = GetConnection())
            {
                conn.Open();
                ExecuteNonQuery(conn, sqlString);
            }
        }

        public void ExecuteSql(List<string> sqlList)
        {
            if (sqlList == null || sqlList.Count <= 0) return;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in sqlList)
                        {
                            ExecuteNonQuery(trans, sql);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ExecuteSqlInFile(string pathToScriptFile, StringBuilder errorBuilder)
        {
            IDbConnection connection;

            if (false == File.Exists(pathToScriptFile))
            {
                throw new Exception("File " + pathToScriptFile + " does not exists");
            }
            using (Stream stream = File.OpenRead(pathToScriptFile))
            {
                var reader = new StreamReader(stream, Encoding.UTF8);

                connection = GetConnection();

                var command = SqlUtils.GetIDbCommand();

                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string sqlString;
                while (null != (sqlString = SqlUtils.ReadNextSqlString(reader)))
                {
                    command.CommandText = sqlString;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorBuilder.Append($@"
                    sql:{sqlString}
                    message:{ex.Message}
                    ");
                    }
                }

                reader.Close();
            }
            connection.Close();
        }

        public void ExecuteSqlInFile(string pathToScriptFile, string tableName, StringBuilder errorBuilder)
        {
            IDbConnection connection;

            if (false == File.Exists(pathToScriptFile))
            {
                throw new Exception("File " + pathToScriptFile + " does not exists");
            }
            using (Stream stream = File.OpenRead(pathToScriptFile))
            {
                var reader = new StreamReader(stream, Encoding.Default);

                connection = GetConnection();

                var command = SqlUtils.GetIDbCommand();

                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string sqlString;
                while (null != (sqlString = SqlUtils.ReadNextSqlString(reader)))
                {
                    sqlString = string.Format(sqlString, tableName);
                    command.CommandText = sqlString;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorBuilder.Append($@"
                    sql:{sqlString}
                    message:{ex.Message}
                    ");
                    }
                }

                reader.Close();
            }
            connection.Close();
        }

        public int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
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
            var count = 0;

            using (var conn = GetConnection())
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

        public int GetIntResult(string sqlString, IDataParameter[] parms)
        {
            var count = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString, parms))
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

        public List<int> GetIntList(string sqlString)
        {
            var list = new List<int>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public List<int> GetIntList(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var list = new List<int>();

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var retval = string.Empty;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        retval = GetString(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return retval;
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

        public List<string> GetStringList(string sqlString)
        {
            var list = new List<string>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetString(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public DateTime GetDateTime(string sqlString)
        {
            var datetime = DateTime.MinValue;
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    datetime = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }

            return datetime;
        }

        public DateTime GetDateTime(string sqlString, IDataParameter[] parms)
        {
            var datetime = DateTime.MinValue;
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    datetime = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }

            return datetime;
        }

        public DataSet GetDataSetByWhereString(string tableEnName, string whereString)
        {
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableEnName, SqlUtils.Asterisk, whereString);
            var dataset = ExecuteDataset(sqlSelect);
            return dataset;
        }

        public IDataReader GetDataReader(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            return string.IsNullOrEmpty(sqlString) ? null : ExecuteReader(connectionString, sqlString);
        }

        public IEnumerable GetDataSource(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public IEnumerable GetDataSource(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = (IEnumerable)ExecuteReader(connectionString, sqlString);
            return enumerable;
        }

        public DataSet GetDataSet(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(connectionString, sqlString);
            return dataset;
        }

        public DataSet GetDataSet(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(sqlString);
            return dataset;
        }

        public void ReadResultsToExtendedAttributes(IDataReader rdr, ExtendedAttributes attributes)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                if (!string.IsNullOrEmpty(value))
                {
                    value = PageUtils.UnFilterSql(value);
                }
                attributes.SetExtendedAttribute(columnName, value);
            }
        }

        public void ReadResultsToNameValueCollection(IDataReader rdr, NameValueCollection attributes)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                if (!string.IsNullOrEmpty(value))
                {
                    value = PageUtils.UnFilterSql(value);
                }
                attributes.Set(columnName, value);
            }
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

            string cmdText = $"SELECT COUNT(*) FROM ({sqlString}) AS t0";
            return GetIntResult(cmdText);
        }

        public string GetPageSqlString(string sqlString, string orderByString, int recordCount, int itemsPerPage, int currentPageIndex)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = recordCount / itemsPerPage;
            var remainder = recordCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            var orderByString2 = orderByString.Replace(" DESC", " DESC2");
            orderByString2 = orderByString2.Replace(" ASC", " DESC");
            orderByString2 = orderByString2.Replace(" DESC2", " ASC");

            if (WebConfigUtils.IsMySql)
            {
                return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderByString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderByString2} LIMIT {recsToRetrieve}
) AS t2 {orderByString}";
            }
            else
            {
                return $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderByString}
    ) AS t1 {orderByString2}
) AS t2 {orderByString}";
            }
        }

        public void Install(StringBuilder errorBuilder)
        {
            var sqlPath = PathUtils.GetInstallSqlFilePath(WebConfigUtils.IsMySql);
            BaiRongDataProvider.DatabaseDao.ExecuteSqlInFile(sqlPath, errorBuilder);
            BaiRongDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();
        }

        public void Upgrade(bool isMySql, StringBuilder errorBuilder)
        {
            var filePathUpgrade = PathUtils.GetUpgradeSqlFilePath(isMySql, false);
            var filePathUpgradeTable = PathUtils.GetUpgradeSqlFilePath(isMySql, true);

            BaiRongDataProvider.DatabaseDao.ExecuteSqlInFile(filePathUpgrade, errorBuilder);

            if (FileUtils.IsFileExists(filePathUpgradeTable))
            {
                try
                {
                    var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDb();
                    foreach (var table in tableList)
                    {
                        BaiRongDataProvider.DatabaseDao.ExecuteSqlInFile(filePathUpgradeTable, table.TableEnName, errorBuilder);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            BaiRongDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();
        }

        public bool ConnectToServer(bool isMySql, string connectionStringWithoutDatabaseName, out List<string> databaseNameList, out string errorMessage)
        {
            databaseNameList = new List<string>();
            try
            {
                if (isMySql)
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
                            databaseNameList.Add(dbName);
                        }
                    }

                    connection.Close();
                }
                else
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
                            databaseNameList.Add(dbName);
                        }
                    }

                    connection.Close();
                }

                errorMessage = "";

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }
    }
}
