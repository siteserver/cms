using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Apis;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata.Compilers;

namespace SiteServer.CMS.Database.Core
{
    public static class SqlDifferences
    {
        public static Compiler GetCompiler(DatabaseType databaseType)
        {
            Compiler compiler = null;

            if (databaseType == DatabaseType.MySql)
            {
                compiler = new MySqlCompiler();
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                if (DatabaseApi.Instance.IsSqlServerGreaterEqual2012)
                {
                    compiler = new SqlServerCompiler();
                }
                else
                {
                    compiler = new SqlServerCompiler
                    {
                        UseLegacyPagination = true
                    };
                }
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                compiler = new PostgresCompiler();
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                compiler = new OracleCompiler();
            }

            return compiler;
        }

        public static IDbConnection GetIDbConnection(DatabaseType databaseType, string connectionString)
        {
            IDbConnection conn = null;

            if (databaseType == DatabaseType.MySql)
            {
                conn = new MySqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                conn = new SqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                conn = new NpgsqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                conn = new OracleConnection(connectionString);
            }

            return conn;
        }

        public static string ColumnIncrement(string columnName, int plusNum = 1)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"IFNULL({columnName}, 0) + {plusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"ISNULL({columnName}, 0) + {plusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"COALESCE({columnName}, 0) + {plusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"COALESCE({columnName}, 0) + {plusNum}";
            }

            return retVal;
        }

        public static string ColumnDecrement(string columnName, int minusNum = 1)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"IFNULL({columnName}, 0) - {minusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"ISNULL({columnName}, 0) - {minusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"COALESCE({columnName}, 0) - {minusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"COALESCE({columnName}, 0) - {minusNum}";
            }

            return retVal;
        }

        public static string GetSqlString(string tableName, IList<string> columnNames = null, string whereSqlString = null, string orderSqlString = null, int offset = 0, int limit = 0, bool distinct = false)
        {
            var select = distinct ? "SELECT DISTINCT" : "SELECT";
            var columns = columnNames != null && columnNames.Count > 0 ? string.Join(", ", columnNames) : "*";

            var retVal = string.Empty;

            if (offset == 0 && limit == 0)
            {
                return $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString}";
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = limit == 0
                    ? $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && DatabaseApi.Instance.IsSqlServerGreaterEqual2012)
            {
                retVal = limit == 0
                    ? $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && !DatabaseApi.Instance.IsSqlServerGreaterEqual2012)
            {
                if (offset == 0)
                {
                    retVal = $"{select} TOP {limit} {columns} FROM {tableName} {whereSqlString} {orderSqlString}";
                }
                else
                {
                    var rowWhere = limit == 0
                        ? $@"WHERE [row_num] > {offset}"
                        : $@"WHERE [row_num] BETWEEN {offset + 1} AND {offset + limit}";

                    retVal = $@"SELECT * FROM (
    {select} {columns}, ROW_NUMBER() OVER ({orderSqlString}) AS [row_num] FROM [{tableName}] {whereSqlString}
) as T {rowWhere}";
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = limit == 0
                    ? $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = limit == 0
                    ? $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }

            return retVal;
        }
    }
}
