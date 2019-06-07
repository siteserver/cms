using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using Dapper;
using Npgsql;
using SqlKata.Compilers;
using SS.CMS.Data.Utils;

[assembly: InternalsVisibleTo("SS.CMS.Data.Tests")]

namespace SS.CMS.Data.DatabaseImpl
{
    internal class PostgreSqlImpl : IDatabaseImpl
    {
        private static IDatabaseImpl _instance;
        public static IDatabaseImpl Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new PostgreSqlImpl();
                return _instance;
            }
        }

        public DbConnection GetConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        public Compiler GetCompiler(string connectionString)
        {
            return new PostgresCompiler();
        }

        public bool IsUseLegacyPagination(string connectionString)
        {
            return false;
        }

        public List<string> GetTableNames(string connectionString)
        {
            IEnumerable<string> tableNames;

            using (var connection = GetConnection(connectionString))
            {
                var sqlString =
                    $"SELECT table_name FROM information_schema.tables WHERE table_catalog = '{connection.Database}' AND table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')";

                if (string.IsNullOrEmpty(sqlString)) return new List<string>();

                tableNames = connection.Query<string>(sqlString);
            }

            return tableNames.Where(tableName => !string.IsNullOrEmpty(tableName)).ToList();
        }

        public string ColumnIncrement(string columnName, int plusNum = 1)
        {
            return $"COALESCE({columnName}, 0) + {plusNum}";
        }

        public string ColumnDecrement(string columnName, int minusNum = 1)
        {
            return $"COALESCE({columnName}, 0) - {minusNum}";
        }

        public string GetAutoIncrementDataType(bool alterTable = false)
        {
            return "SERIAL";
        }

        private string ToColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"{attributeName} bool";
            }
            if (type == DataType.DateTime)
            {
                return $"{attributeName} timestamptz";
            }
            if (type == DataType.Decimal)
            {
                return $"{attributeName} numeric(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"{attributeName} int4";
            }
            if (type == DataType.Text)
            {
                return $"{attributeName} text";
            }
            return $"{attributeName} varchar({length})";
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
            return identifier;
        }

        private DataType ToDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = Utilities.TrimAndToLower(dataTypeStr);
            switch (dataTypeStr)
            {
                case "varchar":
                    dataType = DataType.VarChar;
                    break;
                case "bool":
                    dataType = DataType.Boolean;
                    break;
                case "timestamptz":
                    dataType = DataType.DateTime;
                    break;
                case "numeric":
                    dataType = DataType.Decimal;
                    break;
                case "int4":
                    dataType = DataType.Integer;
                    break;
                case "text":
                    dataType = DataType.Text;
                    break;
            }

            return dataType;
        }

        public List<TableColumn> GetTableColumns(string connectionString, string tableName)
        {
            var list = new List<TableColumn>();

            using (var connection = GetConnection(connectionString))
            {
                var sqlString =
                   $"SELECT COLUMN_NAME AS ColumnName, UDT_NAME AS UdtName, CHARACTER_MAXIMUM_LENGTH AS CharacterMaximumLength, COLUMN_DEFAULT AS ColumnDefault FROM information_schema.columns WHERE table_catalog = '{connection.Database}' AND table_name = '{tableName.ToLower()}' ORDER BY ordinal_position";

                var columns = connection.Query<dynamic>(sqlString);
                foreach (var column in columns)
                {
                    var columnName = column.ColumnName;
                    var udtName = column.UdtName;
                    var characterMaximumLength = column.CharacterMaximumLength;
                    var columnDefault = column.ColumnDefault;

                    var dataType = ToDataType(udtName);
                    var length = characterMaximumLength;

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

                sqlString =
                    $"select column_name AS ColumnName, constraint_name AS ConstraintName from information_schema.key_column_usage where table_catalog = '{connection.Database}' and table_name = '{tableName.ToLower()}';";

                var rows = connection.Query<dynamic>(sqlString);
                foreach (var row in rows)
                {
                    var columnName = row.ColumnName;
                    var constraintName = row.ConstraintName;

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
            }

            return list;
        }

        public string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            return $"ALTER TABLE {GetQuotedIdentifier(tableName)} ADD {columnsSqlString}";
        }
    }
}
