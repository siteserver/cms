using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using Dapper;
using SqlKata.Compilers;
using SS.CMS.Data.Utils;

[assembly: InternalsVisibleTo("SS.CMS.Plugin.Tests")]

namespace SS.CMS.Data.Database
{
    internal class SQLite : IDatabase
    {
        private static IDatabase _instance;
        public static IDatabase Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new SQLite();
                return _instance;
            }
        }

        public IDbConnection GetConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        public Compiler GetCompiler(string connectionString)
        {
            return new SqliteCompiler();
        }

        public List<string> GetTableNames(string connectionString)
        {
            IEnumerable<string> tableNames;

            using (var connection = GetConnection(connectionString))
            {
                var sqlString = "SELECT name FROM sqlite_master WHERE type='table'";

                if (string.IsNullOrEmpty(sqlString)) return new List<string>();

                tableNames = connection.Query<string>(sqlString);
            }

            return tableNames.Where(tableName => !string.IsNullOrEmpty(tableName)).ToList();
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
            if (dataTypeStr.StartsWith("TIMESTAMP("))
            {
                dataType = DataType.DateTime;
            }
            else if (dataTypeStr == "NUMBER")
            {
                dataType = DataType.Integer;
            }
            else if (dataTypeStr == "NCLOB")
            {
                dataType = DataType.Text;
            }
            else if (dataTypeStr == "NVARCHAR2")
            {
                dataType = DataType.VarChar;
            }
            else if (dataTypeStr == "CLOB")
            {
                dataType = DataType.Text;
            }
            else if (dataTypeStr == "VARCHAR2")
            {
                dataType = DataType.VarChar;
            }

            return dataType;
        }

        public List<TableColumn> GetTableColumns(string connectionString, string tableName)
        {
            var list = new List<TableColumn>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                var sqlString =
                    $"PRAGMA table_info('{tableName}')";

                IEnumerable<dynamic> columns = connection.Query<dynamic>(sqlString);

                foreach (var column in columns)
                {
                    var columnName = column.name;
                    var dataType = ToDataType(column.type);
                    var percision = column.DataPrecision;
                    var scale = column.DataScale;
                    var charLength = column.CharLength;
                    var dataDefault = column.DataDefault;

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

                    var isPrimaryKey = Utilities.EqualsIgnoreCase(columnName, "id");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = charLength,
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
