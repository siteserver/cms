using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata.Compilers;
using Datory.Utils;

[assembly: InternalsVisibleTo("Datory.Tests")]

namespace Datory.DatabaseImpl
{
    internal class SqlServerImpl : IDatabaseImpl
    {
        private static IDatabaseImpl _instance;
        public static IDatabaseImpl Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new SqlServerImpl();
                return _instance;
            }
        }

        public DbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public Compiler GetCompiler(string connectionString)
        {
            return new SqlServerCompiler
            {
                UseLegacyPagination = IsUseLegacyPagination(connectionString)
            };
        }

        private static readonly ConcurrentDictionary<string, bool> UseLegacyPagination = new ConcurrentDictionary<string, bool>();

        public bool IsUseLegacyPagination(string connectionString)
        {
            if (UseLegacyPagination.TryGetValue(connectionString, out var useLegacyPagination)) return useLegacyPagination;
            useLegacyPagination = false;

            const string sqlString = "select left(cast(serverproperty('productversion') as varchar), 4)";

            try
            {
                var db = new Database(DatabaseType.SqlServer, connectionString);
                using var connection = db.GetConnection();
                var version = connection.ExecuteScalar<string>(sqlString);

                useLegacyPagination = Utilities.ToDecimal(version) < 11;
            }
            catch
            {
                // ignored
            }

            UseLegacyPagination[connectionString] = useLegacyPagination;

            return useLegacyPagination;
        }

        public async Task<List<string>> GetDatabaseNamesAsync(string connectionString)
        {
            var databaseNames = new List<string>();

            using (var connection = GetConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase("master");

                using (var rdr = await connection.ExecuteReaderAsync("select name from master..sysdatabases order by name asc"))
                {
                    while (rdr.Read())
                    {
                        var dbName = rdr["name"] as string;
                        if (dbName == null) continue;
                        if (dbName != "master" &&
                            dbName != "msdb" &&
                            dbName != "tempdb" &&
                            dbName != "model")
                        {
                            databaseNames.Add(dbName);
                        }
                    }
                }
                connection.Close();
            }

            return databaseNames;
        }

        public async Task<List<string>> GetTableNamesAsync(string connectionString)
        {
            IEnumerable<string> tableNames;

            using (var connection = GetConnection(connectionString))
            {
                var sqlString =
                    $"SELECT name FROM [{connection.Database}]..sysobjects WHERE type = 'U' AND category<>2 ORDER BY Name";

                tableNames = await connection.QueryAsync<string>(sqlString);
            }

            return tableNames != null ? tableNames.Where(tableName => !string.IsNullOrEmpty(tableName)).ToList() : new List<string>();
        }

        public string ColumnIncrement(string columnName, int plusNum = 1)
        {
            return $"ISNULL({columnName}, 0) + {plusNum}";
        }

        public string ColumnDecrement(string columnName, int minusNum = 1)
        {
            return $"ISNULL({columnName}, 0) - {minusNum}";
        }

        public string GetAutoIncrementDataType(bool alterTable = false)
        {
            return "int IDENTITY (1, 1)";
        }

        private string ToColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"[{attributeName}] [bit]";
            }
            if (type == DataType.DateTime)
            {
                return $"[{attributeName}] [datetime]";
            }
            if (type == DataType.Decimal)
            {
                return $"[{attributeName}] [decimal] (18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"[{attributeName}] [int]";
            }
            if (type == DataType.Text)
            {
                return $"[{attributeName}] [nvarchar] (max)";
            }
            return $"[{attributeName}] [nvarchar] ({length})";
        }

        public string GetColumnSqlString(TableColumn tableColumn)
        {
            if (tableColumn.IsIdentity)
            {
                return $@"{tableColumn.AttributeName} {GetAutoIncrementDataType()}";
            }

            return ToColumnString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
        }

        public string GetPrimaryKeySqlString(string tableName, string attributeName)
        {
            return $@"CONSTRAINT PK_{tableName}_{attributeName} PRIMARY KEY ({attributeName})";
        }

        public string GetQuotedIdentifier(string identifier)
        {
            return $"[{identifier}]";
        }

        private DataType ToDataType(string dataTypeStr, string dataLength)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = Utilities.TrimAndToLower(dataTypeStr);
            dataLength = Utilities.TrimAndToLower(dataLength);
            switch (dataTypeStr)
            {
                case "bit":
                    dataType = DataType.Boolean;
                    break;
                case "datetime":
                    dataType = DataType.DateTime;
                    break;
                case "datetime2":
                    dataType = DataType.DateTime;
                    break;
                case "datetimeoffset":
                    dataType = DataType.DateTime;
                    break;
                case "decimal":
                    dataType = DataType.Decimal;
                    break;
                case "int":
                    dataType = DataType.Integer;
                    break;
                case "ntext":
                    dataType = DataType.Text;
                    break;
                case "nvarchar":
                    dataType = dataLength == "max" ? DataType.Text : DataType.VarChar;
                    break;
                case "text":
                    dataType = DataType.Text;
                    break;
                case "varchar":
                    dataType = DataType.VarChar;
                    break;
            }

            return dataType;
        }

        public async Task<List<TableColumn>> GetTableColumnsAsync(string connectionString, string tableName)
        {
            var list = new List<TableColumn>();

            using (var connection = GetConnection(connectionString))
            {
                var databaseName = connection.Database;

                var sqlString =
                $"select id from [{databaseName}]..sysobjects where type = 'U' and category <> 2 and name = '{tableName}'";

                var tableId = await connection.QueryFirstOrDefaultAsync<string>(sqlString);
                if (string.IsNullOrEmpty(tableId)) return new List<TableColumn>();

                var isIdentityExist = false;

                sqlString =
                    $"select C.name AS ColumnName, T.name AS DataTypeName, C.length AS Length, C.colstat AS IsPrimaryKeyInt, case when C.autoval is null then 0 else 1 end AS IsIdentityInt from systypes T, syscolumns C where C.id = {tableId} and C.xtype = T.xusertype order by C.colid";

                var columns = await connection.QueryAsync<dynamic>(sqlString);
                foreach (var column in columns)
                {
                    var columnName = column.ColumnName;
                    if (columnName == "msrepl_tran_version")
                    {
                        continue;
                    }

                    var dataTypeName = column.DataTypeName;
                    var length = column.Length;
                    var dataType = ToDataType(dataTypeName, Convert.ToString(length));
                    length = dataType == DataType.VarChar ? length : 0;
                    var isPrimaryKeyInt = column.IsPrimaryKeyInt;
                    var isIdentityInt = column.IsIdentityInt;

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

                if (!isIdentityExist)
                {
                    sqlString = $"select name from syscolumns where id = object_id(N'{tableName}') and COLUMNPROPERTY(id, name,'IsIdentity')= 1";

                    var clName = connection.QueryFirstOrDefault<string>(sqlString);
                    foreach (var info in list)
                    {
                        if (clName == info.AttributeName)
                        {
                            info.IsIdentity = true;
                        }
                    }
                }
            }

            return list;
        }

        public string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            return $"ALTER TABLE {GetQuotedIdentifier(tableName)} ADD {columnsSqlString}";
        }
    }
}
