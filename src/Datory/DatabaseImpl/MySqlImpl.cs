using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using Datory.Utils;

[assembly: InternalsVisibleTo("Datory.Tests")]

namespace Datory.DatabaseImpl
{
    internal class MySqlImpl : IDatabaseImpl
    {
        private static IDatabaseImpl _instance;
        public static IDatabaseImpl Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new MySqlImpl();
                return _instance;
            }
        }

        public DbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public Compiler GetCompiler(string connectionString)
        {
            return new MySqlCompiler();
        }

        public bool IsUseLegacyPagination(string connectionString)
        {
            return false;
        }

        public async Task<List<string>> GetDatabaseNamesAsync(string connectionString)
        {
            var databaseNames = new List<string>();

            using (var connection = GetConnection(connectionString))
            {
                using var rdr = await connection.ExecuteReaderAsync("show databases");
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
                        databaseNames.Add(dbName);
                    }
                }
            }

            return databaseNames;
        }

        public async Task<List<string>> GetTableNamesAsync(string connectionString)
        {
            IEnumerable<string> tableNames;

            using (var connection = GetConnection(connectionString))
            {
                var sqlString = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{connection.Database}' ORDER BY table_name";

                tableNames = await connection.QueryAsync<string>(sqlString);
            }

            return tableNames != null ? tableNames.Where(tableName => !string.IsNullOrEmpty(tableName)).ToList() : new List<string>();
        }

        public string ColumnIncrement(string columnName, int plusNum = 1)
        {
            return $"IFNULL({columnName}, 0) + {plusNum}";
        }

        public string ColumnDecrement(string columnName, int minusNum = 1)
        {
            return $"IFNULL({columnName}, 0) - {minusNum}";
        }

        public string GetAutoIncrementDataType(bool alterTable = false)
        {
            return alterTable ? "INT AUTO_INCREMENT UNIQUE KEY" : "INT AUTO_INCREMENT";
        }

        private string ToColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"`{attributeName}` tinyint(1)";
            }
            if (type == DataType.DateTime)
            {
                return $"`{attributeName}` datetime";
            }
            if (type == DataType.Decimal)
            {
                return $"`{attributeName}` decimal(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"`{attributeName}` int";
            }
            if (type == DataType.Text)
            {
                return $"`{attributeName}` longtext";
            }
            return $"`{attributeName}` varchar({length})";
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
            return $@"PRIMARY KEY ({attributeName})";
        }

        public string GetQuotedIdentifier(string identifier)
        {
            return $"`{identifier}`";
        }

        private DataType ToDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = Utilities.TrimAndToLower(dataTypeStr);
            switch (dataTypeStr)
            {
                case "bit":
                    dataType = DataType.Boolean;
                    break;
                case "datetime":
                    dataType = DataType.DateTime;
                    break;
                case "decimal":
                    dataType = DataType.Decimal;
                    break;
                case "int":
                    dataType = DataType.Integer;
                    break;
                case "longtext":
                    dataType = DataType.Text;
                    break;
                case "nvarchar":
                    dataType = DataType.VarChar;
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

            using (var connection = new MySqlConnection(connectionString))
            {
                var sqlString =
                    $"select COLUMN_NAME AS ColumnName, DATA_TYPE AS DataType, CHARACTER_MAXIMUM_LENGTH AS DataLength, COLUMN_KEY AS ColumnKey, EXTRA AS Extra from information_schema.columns where table_schema = '{connection.Database}' and table_name = '{tableName}' order by table_name,ordinal_position; ";

                using var rdr = await connection.ExecuteReaderAsync(sqlString);
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
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

            //var columns = database.Connection.Query<dynamic>(sqlString);
            //foreach (var column in columns)
            //{
            //    var columnName = column.ColumnName;
            //    var dataType = ToMySqlDataType(column.DataType);
            //    var dataLength = column.DataLength;

            //    var length = dataLength == null || dataType == DataType.Text ? 0 : dataLength;

            //    var isPrimaryKey = Convert.ToString(column.ColumnKey) == "PRI";
            //    var isIdentity = Convert.ToString(column.Extra) == "auto_increment";

            //    var info = new TableColumn
            //    {
            //        AttributeName = columnName,
            //        DataType = dataType,
            //        DataLength = length,
            //        IsPrimaryKey = isPrimaryKey,
            //        IsIdentity = isIdentity
            //    };
            //    list.Add(info);

            //}

            return list;
        }

        public string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            return $"ALTER TABLE {GetQuotedIdentifier(tableName)} ADD ({columnsSqlString})";
        }
    }
}
