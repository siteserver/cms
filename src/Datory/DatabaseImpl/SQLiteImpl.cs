using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata.Compilers;
using Datory.Utils;

[assembly: InternalsVisibleTo("Datory.Tests")]

namespace Datory.DatabaseImpl
{
    internal class SQLiteImpl : IDatabaseImpl
    {
        private static IDatabaseImpl _instance;
        public static IDatabaseImpl Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new SQLiteImpl();
                return _instance;
            }
        }

        public DbConnection GetConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        public Compiler GetCompiler(string connectionString)
        {
            return new SqliteCompiler();
        }

        public bool IsUseLegacyPagination(string connectionString)
        {
            return false;
        }

        public async Task<List<string>> GetDatabaseNamesAsync(string connectionString)
        {
            return await Task.FromResult(new List<string>());
        }

        public async Task<List<string>> GetTableNamesAsync(string connectionString)
        {
            IEnumerable<string> tableNames;

            using (var connection = GetConnection(connectionString))
            {
                var sqlString = "SELECT name FROM sqlite_master WHERE type='table'";

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
            return "INTEGER PRIMARY KEY";
        }

        public string GetColumnSqlString(TableColumn tableColumn)
        {
            if (tableColumn.IsIdentity)
            {
                return $@"{tableColumn.AttributeName} {GetAutoIncrementDataType()}";
            }

            return ToColumnString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
        }

        private string ToColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"{attributeName} INT2";
            }
            if (type == DataType.DateTime)
            {
                return $"{attributeName} DATETIME";
            }
            if (type == DataType.Decimal)
            {
                return $"{attributeName} DECIMAL(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"{attributeName} INTEGER";
            }
            if (type == DataType.Text)
            {
                return $"{attributeName} TEXT";
            }
            return $"{attributeName} NVARCHAR({length})";
        }

        public string GetPrimaryKeySqlString(string tableName, string attributeName)
        {
            return GetAutoIncrementDataType();
        }

        public string GetQuotedIdentifier(string identifier)
        {
            return identifier;
        }

        private DataType ToDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = Utilities.TrimAndToUpper(dataTypeStr);
            if (dataTypeStr.StartsWith("REAL"))
            {
                dataType = DataType.Decimal;
            }
            else if (dataTypeStr == "INTEGER")
            {
                dataType = DataType.Integer;
            }
            else if (dataTypeStr == "TEXT")
            {
                dataType = DataType.Text;
            }
            else if (dataTypeStr == "DATETIME")
            {
                dataType = DataType.DateTime;
            }

            return dataType;
        }

        public async Task<List<TableColumn>> GetTableColumnsAsync(string connectionString, string tableName)
        {
            var list = new List<TableColumn>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                var sqlString =
                    $"PRAGMA table_info('{tableName}')";

                IEnumerable<dynamic> columns = await connection.QueryAsync<dynamic>(sqlString);

                foreach (var column in columns)
                {
                    var columnName = column.name;
                    var dataType = ToDataType(column.type);

                    var isPrimaryKey = Utilities.EqualsIgnoreCase(columnName, "id");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = 2000,
                        IsPrimaryKey = isPrimaryKey,
                        IsIdentity = isPrimaryKey
                    };
                    list.Add(info);
                }
            }

            return list;
        }

        public string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            return $"ALTER TABLE {GetQuotedIdentifier(tableName)} ADD " + columnsSqlString;
        }
    }
}
