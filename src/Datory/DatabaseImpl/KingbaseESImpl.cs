using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata.Compilers;
using Datory.Utils;
using Kdbndp;

[assembly: InternalsVisibleTo("Datory.Tests")]

namespace Datory.DatabaseImpl
{
    internal class KingbaseESImpl : IDatabaseImpl
    {
        private static IDatabaseImpl _instance;
        public static IDatabaseImpl Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new KingbaseESImpl();
                return _instance;
            }
        }

        public string GetConnectionString(string server, bool isDefaultPort, int port, string userName, string password, string databaseName)
        {
            var connectionString = $"Server={server};";

            if (!isDefaultPort && port > 0)
            {
                connectionString += $"Port={port};";
            }
            else
            {
                connectionString += "Port=54321;";
            }
            connectionString += $"User Id={userName};Password={password};";
            connectionString += $"Database={databaseName};";

            return connectionString;
        }

        public DbConnection GetConnection(string connectionString)
        {
            return new KdbndpConnection(connectionString);
        }

        public Compiler GetCompiler(string connectionString)
        {
            return new PostgresCompiler();
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
                using var rdr = await connection.ExecuteReaderAsync("select datname from pg_database where datistemplate = false order by datname asc");
                while (rdr.Read())
                {
                    var dbName = rdr["datname"] as string;
                    if (dbName == null) continue;

                    databaseNames.Add(dbName);
                }
            }

            return databaseNames;
        }

        public async Task<bool> IsTableExistsAsync(string connectionString, string tableName)
        {
            bool exists;
            var databaseName = Utilities.GetConnectionStringDatabase(connectionString);
            tableName = Utilities.FilterSql(tableName);

            try
            {
                var sql = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_catalog = '{databaseName}' AND table_name = '{tableName}'";

                using var connection = GetConnection(connectionString);
                exists = await connection.ExecuteScalarAsync<int>(sql) == 1;
            }
            catch
            {
                try
                {
                    var sql = $"select 1 from {tableName} where 1 = 0";

                    using var connection = GetConnection(connectionString);
                    exists = await connection.ExecuteScalarAsync<int>(sql) == 1;
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }

        public async Task<List<string>> GetTableNamesAsync(string connectionString)
        {
            IEnumerable<string> tableNames;

            using (var connection = GetConnection(connectionString))
            {
                var sqlString =
                    $"SELECT table_name FROM information_schema.tables WHERE table_catalog = '{connection.Database}' AND table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')";

                tableNames = await connection.QueryAsync<string>(sqlString);
            }

            return tableNames != null ? tableNames.Where(tableName => !string.IsNullOrEmpty(tableName)).ToList() : new List<string>();
        }

        public string ColumnIncrement(string columnName, int plusNum = 1)
        {
            return $"IFNULL({GetQuotedIdentifier(columnName)}, 0) + {plusNum}";
        }

        public string ColumnDecrement(string columnName, int minusNum = 1)
        {
            return $"IFNULL({GetQuotedIdentifier(columnName)}, 0) - {minusNum}";
        }

        public string GetAutoIncrementDataType(bool alterTable = false)
        {
            return "integer AUTO_INCREMENT";
        }

        private string ToColumnString(DataType type, string columnName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"{columnName} boolean";
            }
            if (type == DataType.DateTime)
            {
                return $"{columnName} timestamptz";
            }
            if (type == DataType.Decimal)
            {
                return $"{columnName} dec(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"{columnName} integer";
            }
            if (type == DataType.Text)
            {
                return $"{columnName} text";
            }
            return $"{columnName} varchar({length})";
        }

        public string GetColumnSqlString(TableColumn tableColumn)
        {
            var columnName = GetQuotedIdentifier(tableColumn.AttributeName);
            if (tableColumn.IsIdentity)
            {
                return $@"{columnName} {GetAutoIncrementDataType()}";
            }

            return ToColumnString(tableColumn.DataType, columnName, tableColumn.DataLength);
        }

        public string GetPrimaryKeySqlString(string tableName, string attributeName)
        {
            return $@"PRIMARY KEY ({attributeName})";
        }

        public string GetQuotedIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier) || identifier == "*")
            {
                return identifier;
            }

            return $@"""{identifier}""";
        }

        private DataType ToDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = Utilities.TrimAndToLower(dataTypeStr);
            switch (dataTypeStr)
            {
                case "boolean":
                    dataType = DataType.Boolean;
                    break;
                case "timestamptz":
                    dataType = DataType.DateTime;
                    break;
                case "dec":
                    dataType = DataType.Decimal;
                    break;
                case "integer":
                    dataType = DataType.Integer;
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
            tableName = Utilities.FilterSql(tableName);

            using (var connection = GetConnection(connectionString))
            {
                var sqlString =
                   $@"SELECT COLUMN_NAME AS ""ColumnName"", UDT_NAME AS ""UdtName"", CHARACTER_MAXIMUM_LENGTH AS ""CharacterMaximumLength"", COLUMN_DEFAULT AS ""ColumnDefault"" FROM information_schema.columns WHERE table_catalog = '{connection.Database}' AND table_name = '{tableName}' ORDER BY ordinal_position";

                var columns = await connection.QueryAsync<dynamic>(sqlString);
                foreach (var column in columns)
                {
                    var columnName = column.ColumnName;
                    var udtName = column.UdtName;
                    var characterMaximumLength = column.CharacterMaximumLength;
                    var columnDefault = column.ColumnDefault;

                    var dataType = ToDataType(udtName);
                    var length = characterMaximumLength;

                    var isIdentity = columnDefault != null && columnDefault.StartsWith("nextval(");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        IsPrimaryKey = false,
                        IsIdentity = isIdentity
                    };
                    if (length != null)
                    {
                        info.DataLength = length;
                    }
                    list.Add(info);
                }

                sqlString =
                    $@"select column_name AS ""ColumnName"", constraint_name AS ""ConstraintName"" from information_schema.key_column_usage where table_catalog = '{connection.Database}' and table_name = '{tableName}';";

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
            tableName = GetQuotedIdentifier(Utilities.FilterSql(tableName));
            return $"ALTER TABLE {tableName} ADD {columnsSqlString}";
        }

        public string GetOrderByRandomString()
        {
            return "random()";
        }

        public string GetInStr(string columnName, string inStr)
        {
            inStr = Utilities.FilterSql(inStr);
            columnName = GetQuotedIdentifier(columnName);
            
            return $"POSITION('{inStr}' IN {columnName}) > 0";
        }

        public string GetNotInStr(string columnName, string inStr)
        {
            inStr = Utilities.FilterSql(inStr);
            columnName = GetQuotedIdentifier(columnName);
            
            return $"POSITION('{inStr}' IN {columnName}) = 0";
        }
    }
}
