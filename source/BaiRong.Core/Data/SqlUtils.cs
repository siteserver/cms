using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Data
{
    public static class SqlUtils
    {
        public const string Asterisk = "*";

        public static string GetConnectionString(EDatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string database)
        {
            string connectionString;
            switch (databaseType)
            {
                case EDatabaseType.MySql:
                    connectionString = $"Server={server};";
                    if (!isDefaultPort && port > 0)
                    {
                        connectionString += $"Port={port};";
                    }
                    connectionString += $"Uid={userName};Pwd={password};";
                    if (!string.IsNullOrEmpty(database))
                    {
                        connectionString += $"Database={database};";
                    }
                    break;
                case EDatabaseType.SqlServer:
                    connectionString = $"Server={server};";
                    if (!isDefaultPort && port > 0)
                    {
                        connectionString += $"Port={port};";
                    }
                    connectionString += $"Uid={userName};Pwd={password};";
                    if (!string.IsNullOrEmpty(database))
                    {
                        connectionString += $"Database={database};";
                    }
                    break;
                case EDatabaseType.PostgreSql:
                    connectionString = $"Host={server};";
                    if (!isDefaultPort && port > 0)
                    {
                        connectionString += $"Port={port};";
                    }
                    connectionString += $"Username={userName};Password={password};";
                    if (!string.IsNullOrEmpty(database))
                    {
                        connectionString += $"Database={database};";
                    }
                    break;
                case EDatabaseType.Oracle:
                    port = !isDefaultPort && port > 0 ? port: 1521;
                    database = string.IsNullOrEmpty(database)
                        ? string.Empty
                        : $"(CONNECT_DATA=(SERVICE_NAME={database}))";
                    connectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port})){database});User ID={userName};Password={password};";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }
            return connectionString;
        }

        public static IDbConnection GetIDbConnection(EDatabaseType databaseType, string connectionString)
        {
            IDbConnection conn;

            switch (databaseType)
            {
                case EDatabaseType.MySql:
                    conn = new MySqlConnection(connectionString);
                    break;
                case EDatabaseType.SqlServer:
                    conn = new SqlConnection(connectionString);
                    break;
                case EDatabaseType.PostgreSql:
                    conn = new NpgsqlConnection(connectionString);
                    break;
                case EDatabaseType.Oracle:
                    conn = new OracleConnection(connectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }

            return conn;
        }

        public static IDbCommand GetIDbCommand()
        {
            IDbCommand command;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    command = new MySqlCommand();
                    break;
                case EDatabaseType.SqlServer:
                    command = new SqlCommand();
                    break;
                case EDatabaseType.PostgreSql:
                    command = new NpgsqlCommand();
                    break;
                case EDatabaseType.Oracle:
                    command = new OracleCommand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return command;
        }

        public static IDbDataAdapter GetIDbDataAdapter(string text, string connectionString)
        {
            IDbDataAdapter adapter;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    adapter = new MySqlDataAdapter(text, connectionString);
                    break;
                case EDatabaseType.SqlServer:
                    adapter = new SqlDataAdapter(text, connectionString);
                    break;
                case EDatabaseType.PostgreSql:
                    adapter = new NpgsqlDataAdapter(text, connectionString);
                    break;
                case EDatabaseType.Oracle:
                    adapter = new OracleDataAdapter(text, connectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return adapter;
        }

        public static IDbDataAdapter GetIDbDataAdapter()
        {
            IDbDataAdapter adapter;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    adapter = new MySqlDataAdapter();
                    break;
                case EDatabaseType.SqlServer:
                    adapter = new SqlDataAdapter();
                    break;
                case EDatabaseType.PostgreSql:
                    adapter = new NpgsqlDataAdapter();
                    break;
                case EDatabaseType.Oracle:
                    adapter = new OracleDataAdapter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return adapter;
        }

        public static void FillDataAdapterWithDataTable(IDbDataAdapter adapter, DataTable table)
        {
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    ((MySqlDataAdapter)adapter).Fill(table);
                    break;
                case EDatabaseType.SqlServer:
                    ((SqlDataAdapter)adapter).Fill(table);
                    break;
                case EDatabaseType.PostgreSql:
                    ((NpgsqlDataAdapter)adapter).Fill(table);
                    break;
                case EDatabaseType.Oracle:
                    ((OracleDataAdapter)adapter).Fill(table);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IDbDataParameter GetIDbDataParameter(string parameterName, DataType dataType, int size)
        {
            IDbDataParameter parameter;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType), size);
                    break;
                case EDatabaseType.SqlServer:
                    parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType), size);
                    break;
                case EDatabaseType.PostgreSql:
                    parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType), size);
                    break;
                case EDatabaseType.Oracle:
                    parameter = new OracleParameter(parameterName, ToOracleDbType(dataType), size);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return parameter;
        }

        public static IDbDataParameter GetIDbDataParameter(string parameterName, DataType dataType)
        {
            IDbDataParameter parameter;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType));
                    break;
                case EDatabaseType.SqlServer:
                    parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType));
                    break;
                case EDatabaseType.PostgreSql:
                    parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType));
                    break;
                case EDatabaseType.Oracle:
                    parameter = new OracleParameter(parameterName, ToOracleDbType(dataType));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return parameter;
        }

        public static string GetInStr(string columnName, string inStr)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"INSTR({columnName}, '{inStr}') > 0";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"CHARINDEX('{inStr}', {columnName}) > 0";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"POSITION('{inStr}' IN {columnName}) > 0";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"INSTR({columnName}, '{inStr}') > 0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return retval;
        }

        public static string GetInStrReverse(string inStr, string columnName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"INSTR('{inStr}', {columnName}) > 0";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"CHARINDEX({columnName}, '{inStr}') > 0";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"POSITION({columnName} IN '{inStr}') > 0";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"INSTR('{inStr}', {columnName}) > 0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return retval;
        }

        public static string GetNotInStr(string columnName, string inStr)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"INSTR({columnName}, '{inStr}') = 0";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"CHARINDEX('{inStr}', {columnName}) = 0";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"POSITION('{inStr}' IN {columnName}) = 0";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"INSTR({columnName}, '{inStr}') = 0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return retval;
        }

        public static string GetNotNullAndEmpty(string columnName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"LENGTH(IFNULL({columnName},'')) > 0";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATALENGTH({columnName}) > 0";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"LENGTH(COALESCE({columnName}, '')) > 0";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"LENGTH(COALESCE({columnName}, '')) > 0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetNullOrEmpty(string columnName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"LENGTH(IFNULL({columnName},'')) = 0";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATALENGTH({columnName}) = 0";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"LENGTH(COALESCE({columnName}, '')) = 0";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"LENGTH(COALESCE({columnName}, '')) = 0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            string retval = $"SELECT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retval;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"SELECT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
                    break;
                case EDatabaseType.Oracle:
                    //retval = $@"SELECT {columns} FROM (SELECT {columns} FROM {tableName} {whereString} {orderString}) WHERE ROWNUM <= {topN} ORDER BY ROWNUM ASC";
                    retval = $@"SELECT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetTopSqlString(string sqlString, string orderString, int topN)
        {
            string retval = $"SELECT * FROM ({sqlString}) {orderString}";
            if (topN <= 0) return retval;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"SELECT TOP {topN} * FROM ({sqlString}) {orderString}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
                    break;
                case EDatabaseType.Oracle:
                    //retval = $@"SELECT * FROM (SELECT * FROM ({sqlString}) {orderString}) WHERE ROWNUM <= {topN} ORDER BY ROWNUM ASC";
                    retval = $@"SELECT * FROM ({sqlString}) {orderString} FETCH FIRST {topN} ROWS ONLY";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetPageSqlString(string sqlString, string orderString, int itemsPerPage, int currentPageIndex, int pageCount, int recordsInLastPage)
        {
            string retval;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
            {
                recsToRetrieve = recordsInLastPage;
            }

            orderString = orderString.ToUpper();
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
                    break;
                case EDatabaseType.Oracle:
                    retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} FETCH FIRST {itemsPerPage * (currentPageIndex + 1)} ROWS ONLY
    ) AS t1 {orderStringReverse} FETCH FIRST {recsToRetrieve} ROWS ONLY
) AS t2 {orderString}";
                    //                    retval = $@"
                    //SELECT * FROM (
                    //    SELECT * FROM (
                    //        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} AS t0 {orderString}
                    //    ) WHERE ROWNUM <= {recsToRetrieve} AS t1 {orderStringReverse}
                    //) AS t2 {orderString}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetDistinctTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            var retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retval;

            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"SELECT DISTINCT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
                    break;
                case EDatabaseType.Oracle:
                    //retval = $@"SELECT {columns} FROM (SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString}) WHERE ROWNUM <= {topN} ORDER BY ROWNUM ASC";
                    retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetInTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            var builder = new StringBuilder();
            foreach (var column in TranslateUtils.StringCollectionToStringList(columns))
            {
                builder.Append($"T.{column}, ");
            }
            builder.Length = builder.Length - 2;
            return
                $"SELECT {builder} FROM ({GetTopSqlString(tableName, columns, whereString, orderString, topN)}) AS T";
        }

        public static string GetColumnSqlString(DataType dataType, string attributeName, int length)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = ToMySqlColumnString(dataType, attributeName, length);
                    break;
                case EDatabaseType.SqlServer:
                    retval = ToSqlServerColumnString(dataType, attributeName, length);
                    break;
                case EDatabaseType.PostgreSql:
                    retval = ToPostgreColumnString(dataType, attributeName, length);
                    break;
                case EDatabaseType.Oracle:
                    retval = ToOracleColumnString(dataType, attributeName, length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"ALTER TABLE `{tableName}` ADD ({columnsSqlString})";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"ALTER TABLE [{tableName}] ADD {columnsSqlString}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"ALTER TABLE {tableName} ADD {columnsSqlString}";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"ALTER TABLE {tableName} ADD {columnsSqlString}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static List<string> GetDropColumnsSqlString(string tableName, string attributeName)
        {
            var sqlList = new List<string>();
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
                    break;
                case EDatabaseType.SqlServer:
                    var defaultConstraintName = BaiRongDataProvider.DatabaseDao.GetSqlServerDefaultConstraintName(tableName, attributeName);
                    if (!string.IsNullOrEmpty(defaultConstraintName))
                    {
                        sqlList.Add($"ALTER TABLE [{tableName}] DROP CONSTRAINT [{defaultConstraintName}]");
                    }
                    sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
                    break;
                case EDatabaseType.PostgreSql:
                    sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
                    break;
                case EDatabaseType.Oracle:
                    sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return sqlList;
        }

        public static string GetAutoIncrementDataType()
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = "INT AUTO_INCREMENT";
                    break;
                case EDatabaseType.SqlServer:
                    retval = "int IDENTITY (1, 1)";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = "SERIAL";
                    break;
                case EDatabaseType.Oracle:
                    retval = "NUMBER GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1)";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetCreateAuxiliaryTableSqlString(string tableName)
        {
            var columnSqlStringList = new List<string>();

            var tableMetadataInfoList = TableManager.GetTableMetadataInfoList(tableName);
            if (tableMetadataInfoList.Count > 0)
            {
                foreach (var metadataInfo in tableMetadataInfoList)
                {
                    var columnSql = GetColumnSqlString(metadataInfo.DataType, metadataInfo.AttributeName, metadataInfo.DataLength);
                    if (!string.IsNullOrEmpty(columnSql))
                    {
                        columnSqlStringList.Add(columnSql);
                    }
                }
            }

            var sqlBuilder = new StringBuilder();

            //添加默认字段
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    sqlBuilder.Append($@"CREATE TABLE `{tableName}` (");
                    sqlBuilder.Append($@"
`{nameof(ContentInfo.Id)}` INT AUTO_INCREMENT,
`{nameof(ContentInfo.NodeId)}` INT,
`{nameof(ContentInfo.PublishmentSystemId)}` INT,
`{nameof(ContentInfo.AddUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditUserName)}` VARCHAR(255),
`{nameof(ContentInfo.WritingUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditDate)}` DATETIME,
`{nameof(ContentInfo.Taxis)}` INT,
`{nameof(ContentInfo.ContentGroupNameCollection)}` VARCHAR(255),
`{nameof(ContentInfo.Tags)}` VARCHAR(255),
`{nameof(ContentInfo.SourceId)}` INT,
`{nameof(ContentInfo.ReferenceId)}` INT,
`{nameof(ContentInfo.IsChecked)}` VARCHAR(18),
`{nameof(ContentInfo.CheckedLevel)}` INT,
`{nameof(ContentInfo.Comments)}` INT,
`{nameof(ContentInfo.Photos)}` INT,
`{nameof(ContentInfo.Hits)}` INT,
`{nameof(ContentInfo.HitsByDay)}` INT,
`{nameof(ContentInfo.HitsByWeek)}` INT,
`{nameof(ContentInfo.HitsByMonth)}` INT,
`{nameof(ContentInfo.LastHitsDate)}` DATETIME,
`{nameof(ContentInfo.SettingsXml)}` LONGTEXT,
`{nameof(ContentInfo.Title)}` VARCHAR(255),
`{nameof(ContentInfo.IsTop)}` VARCHAR(18),
`{nameof(ContentInfo.IsRecommend)}` VARCHAR(18),
`{nameof(ContentInfo.IsHot)}` VARCHAR(18),
`{nameof(ContentInfo.IsColor)}` VARCHAR(18),
`{nameof(ContentInfo.AddDate)}` DATETIME,
");
                    break;
                case EDatabaseType.SqlServer:
                    sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                    sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} int IDENTITY (1, 1),
{nameof(ContentInfo.NodeId)} int NULL,
{nameof(ContentInfo.PublishmentSystemId)} int NULL,
{nameof(ContentInfo.AddUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.WritingUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditDate)} datetime NULL,
{nameof(ContentInfo.Taxis)} int NULL,
{nameof(ContentInfo.ContentGroupNameCollection)} nvarchar (255) NULL,
{nameof(ContentInfo.Tags)} nvarchar (255) NULL,
{nameof(ContentInfo.SourceId)} int NULL,
{nameof(ContentInfo.ReferenceId)} int NULL,
{nameof(ContentInfo.IsChecked)} nvarchar (18) NULL,
{nameof(ContentInfo.CheckedLevel)} int NULL,
{nameof(ContentInfo.Comments)} int NULL,
{nameof(ContentInfo.Photos)} int NULL,
{nameof(ContentInfo.Hits)} int NULL,
{nameof(ContentInfo.HitsByDay)} int NULL,
{nameof(ContentInfo.HitsByWeek)} int NULL,
{nameof(ContentInfo.HitsByMonth)} int NULL,
{nameof(ContentInfo.LastHitsDate)} datetime NULL,
{nameof(ContentInfo.SettingsXml)} ntext NULL,
{nameof(ContentInfo.Title)} nvarchar (255) NULL,
{nameof(ContentInfo.IsTop)} nvarchar (18) NULL,
{nameof(ContentInfo.IsRecommend)} nvarchar (18) NULL,
{nameof(ContentInfo.IsHot)} nvarchar (18) NULL,
{nameof(ContentInfo.IsColor)} nvarchar (18) NULL,
{nameof(ContentInfo.AddDate)} datetime NULL,
");
                    break;
                case EDatabaseType.PostgreSql:
                    sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                    sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} SERIAL,
{nameof(ContentInfo.NodeId)} int4 NULL,
{nameof(ContentInfo.PublishmentSystemId)} int4 NULL,
{nameof(ContentInfo.AddUserName)} varchar (255) NULL,
{nameof(ContentInfo.LastEditUserName)} varchar (255) NULL,
{nameof(ContentInfo.WritingUserName)} varchar (255) NULL,
{nameof(ContentInfo.LastEditDate)} timestamptz NULL,
{nameof(ContentInfo.Taxis)} int4 NULL,
{nameof(ContentInfo.ContentGroupNameCollection)} varchar (255) NULL,
{nameof(ContentInfo.Tags)} varchar (255) NULL,
{nameof(ContentInfo.SourceId)} int4 NULL,
{nameof(ContentInfo.ReferenceId)} int4 NULL,
{nameof(ContentInfo.IsChecked)} varchar (18) NULL,
{nameof(ContentInfo.CheckedLevel)} int4 NULL,
{nameof(ContentInfo.Comments)} int4 NULL,
{nameof(ContentInfo.Photos)} int4 NULL,
{nameof(ContentInfo.Hits)} int4 NULL,
{nameof(ContentInfo.HitsByDay)} int4 NULL,
{nameof(ContentInfo.HitsByWeek)} int4 NULL,
{nameof(ContentInfo.HitsByMonth)} int4 NULL,
{nameof(ContentInfo.LastHitsDate)} timestamptz NULL,
{nameof(ContentInfo.SettingsXml)} text NULL,
{nameof(ContentInfo.Title)} varchar (255) NULL,
{nameof(ContentInfo.IsTop)} varchar (18) NULL,
{nameof(ContentInfo.IsRecommend)} varchar (18) NULL,
{nameof(ContentInfo.IsHot)} varchar (18) NULL,
{nameof(ContentInfo.IsColor)} varchar (18) NULL,
{nameof(ContentInfo.AddDate)} timestamptz NULL,
");
                    break;
                case EDatabaseType.Oracle:
                    sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                    sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} NUMBER GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1),
{nameof(ContentInfo.NodeId)} number NULL,
{nameof(ContentInfo.PublishmentSystemId)} number NULL,
{nameof(ContentInfo.AddUserName)} nvarchar2(255) NULL,
{nameof(ContentInfo.LastEditUserName)} nvarchar2(255) NULL,
{nameof(ContentInfo.WritingUserName)} nvarchar2(255) NULL,
{nameof(ContentInfo.LastEditDate)} timestamp(6) with time zone NULL,
{nameof(ContentInfo.Taxis)} number NULL,
{nameof(ContentInfo.ContentGroupNameCollection)} nvarchar2(255) NULL,
{nameof(ContentInfo.Tags)} nvarchar2(255) NULL,
{nameof(ContentInfo.SourceId)} number NULL,
{nameof(ContentInfo.ReferenceId)} number NULL,
{nameof(ContentInfo.IsChecked)} nvarchar2(18) NULL,
{nameof(ContentInfo.CheckedLevel)} number NULL,
{nameof(ContentInfo.Comments)} number NULL,
{nameof(ContentInfo.Photos)} number NULL,
{nameof(ContentInfo.Hits)} number NULL,
{nameof(ContentInfo.HitsByDay)} number NULL,
{nameof(ContentInfo.HitsByWeek)} number NULL,
{nameof(ContentInfo.HitsByMonth)} number NULL,
{nameof(ContentInfo.LastHitsDate)} timestamp(6) with time zone NULL,
{nameof(ContentInfo.SettingsXml)} nclob NULL,
{nameof(ContentInfo.Title)} nvarchar2(255) NULL,
{nameof(ContentInfo.IsTop)} nvarchar2(18) NULL,
{nameof(ContentInfo.IsRecommend)} nvarchar2(18) NULL,
{nameof(ContentInfo.IsHot)} nvarchar2(18) NULL,
{nameof(ContentInfo.IsColor)} nvarchar2(18) NULL,
{nameof(ContentInfo.AddDate)} timestamp(6) with time zone NULL,
");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //添加后台定义字段
            foreach (var sqlString in columnSqlStringList)
            {
                sqlBuilder.Append(sqlString).Append(@",");
            }

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append($@"
PRIMARY KEY ({nameof(ContentInfo.Id)})
)
GO
CREATE INDEX `IX_{tableName}` ON `{tableName}`(`{nameof(ContentInfo.IsTop)}` DESC, `{nameof(ContentInfo.Taxis)}` DESC, `{nameof(ContentInfo.Id)}` DESC)
GO
CREATE INDEX `IX_{tableName}_Taxis` ON `{tableName}`(`{nameof(ContentInfo.Taxis)}` DESC)
GO");
            }
            else
            {
                sqlBuilder.Append($@"
CONSTRAINT PK_{tableName} PRIMARY KEY ({nameof(ContentInfo.Id)})
)
GO
CREATE INDEX IX_{tableName} ON {tableName}({nameof(ContentInfo.IsTop)} DESC, {nameof(ContentInfo.Taxis)} DESC, {nameof(ContentInfo.Id)} DESC)
GO
CREATE INDEX IX_{tableName}_Taxis ON {tableName}({nameof(ContentInfo.Taxis)} DESC)
GO");
            }

            return sqlBuilder.ToString();
        }

        public static string GetDefaultDateString()
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = "now()";
                    break;
                case EDatabaseType.SqlServer:
                    retval = "getdate()";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = "current_timestamp";
                    break;
                case EDatabaseType.Oracle:
                    retval = "sysdate";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static DataType ToDataType(EDatabaseType databaseType, string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            switch (databaseType)
            {
                case EDatabaseType.MySql:
                    dataTypeStr = dataTypeStr.ToLower().Trim();
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
                    break;
                case EDatabaseType.SqlServer:
                    dataTypeStr = dataTypeStr.ToLower().Trim();
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
                        case "ntext":
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
                    break;
                case EDatabaseType.PostgreSql:
                    dataTypeStr = dataTypeStr.ToLower().Trim();
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
                    break;
                case EDatabaseType.Oracle:
                    dataTypeStr = dataTypeStr.ToUpper().Trim();
                    switch (dataTypeStr)
                    {
                        case "TIMESTAMP(6)":
                            dataType = DataType.DateTime;
                            break;
                        case "TIMESTAMP(8)":
                            dataType = DataType.DateTime;
                            break;
                        case "NUMBER":
                            dataType = DataType.Integer;
                            break;
                        case "NCLOB":
                            dataType = DataType.Text;
                            break;
                        case "NVARCHAR2":
                            dataType = DataType.VarChar;
                            break;
                        case "CLOB":
                            dataType = DataType.Text;
                            break;
                        case "VARCHAR2":
                            dataType = DataType.VarChar;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }

            return dataType;
        }

        public static SqlDbType ToSqlServerDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return SqlDbType.Bit;
            }
            if (type == DataType.DateTime)
            {
                return SqlDbType.DateTime;
            }
            if (type == DataType.Decimal)
            {
                return SqlDbType.Decimal;
            }
            if (type == DataType.Integer)
            {
                return SqlDbType.Int;
            }
            if (type == DataType.Text)
            {
                return SqlDbType.NText;
            }
            if (type == DataType.VarChar)
            {
                return SqlDbType.NVarChar;
            }
            return SqlDbType.VarChar;
        }

        public static MySqlDbType ToMySqlDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return MySqlDbType.Bit;
            }
            if (type == DataType.DateTime)
            {
                return MySqlDbType.DateTime;
            }
            if (type == DataType.Decimal)
            {
                return MySqlDbType.Decimal;
            }
            if (type == DataType.Integer)
            {
                return MySqlDbType.Int32;
            }
            if (type == DataType.Text)
            {
                return MySqlDbType.LongText;
            }
            if (type == DataType.VarChar)
            {
                return MySqlDbType.VarString;
            }

            return MySqlDbType.VarString;
        }

        public static NpgsqlDbType ToNpgsqlDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return NpgsqlDbType.Boolean;
            }
            if (type == DataType.DateTime)
            {
                return NpgsqlDbType.TimestampTZ;
            }
            if (type == DataType.Decimal)
            {
                return NpgsqlDbType.Numeric;
            }
            if (type == DataType.Integer)
            {
                return NpgsqlDbType.Integer;
            }
            if (type == DataType.Text)
            {
                return NpgsqlDbType.Text;
            }
            return NpgsqlDbType.Varchar;
        }

        public static OracleDbType ToOracleDbType(DataType type)
        {
            switch (type)
            {
                case DataType.Integer:
                    return OracleDbType.Int32;
                case DataType.Decimal:
                    return OracleDbType.Decimal;
                case DataType.VarChar:
                    return OracleDbType.NVarchar2;
                case DataType.Text:
                    return OracleDbType.NClob;
                case DataType.Boolean:
                    return OracleDbType.Int32;
                case DataType.DateTime:
                    return OracleDbType.TimeStampTZ;
            }
            return OracleDbType.NVarchar2;
        }

        public static string ToMySqlColumnString(DataType dataType, string attributeName, int length)
        {
            string retval;
            switch (dataType)
            {

                case DataType.Integer:
                    retval = $"`{attributeName}` int";
                    break;
                case DataType.Decimal:
                    retval = $"`{attributeName}` decimal(18, 2)";
                    break;
                case DataType.VarChar:
                    retval = $"`{attributeName}` varchar({length})";
                    break;
                case DataType.Text:
                    retval = $"`{attributeName}` longtext";
                    break;
                case DataType.Boolean:
                    retval = $"`{attributeName}` tinyint(1)";
                    break;
                case DataType.DateTime:
                    retval = $"`{attributeName}` datetime";
                    break;
                default:
                    retval = $"`{attributeName}` varchar({length})";
                    break;
            }
            return retval;
        }

        public static string ToSqlServerColumnString(DataType dataType, string attributeName, int length)
        {
            string retval;
            switch (dataType)
            {
                case DataType.Integer:
                    retval = $"[{attributeName}] [int]";
                    break;
                case DataType.Decimal:
                    retval = $"[{attributeName}] [decimal] (18, 2)";
                    break;
                case DataType.VarChar:
                    retval = $"[{attributeName}] [nvarchar] ({length})";
                    break;
                case DataType.Text:
                    retval = $"[{attributeName}] [ntext]";
                    break;
                case DataType.Boolean:
                    retval = $"[{attributeName}] [bit]";
                    break;
                case DataType.DateTime:
                    retval = $"[{attributeName}] [datetime]";
                    break;
                default:
                    retval = $"[{attributeName}] [nvarchar] ({length})";
                    break;
            }

            retval += " NULL";
            return retval;
        }

        public static string ToPostgreColumnString(DataType dataType, string attributeName, int length)
        {
            string retval;
            switch (dataType)
            {
                case DataType.Integer:
                    retval = $"{attributeName} int4";
                    break;
                case DataType.Decimal:
                    retval = $"{attributeName} numeric(18, 2)";
                    break;
                case DataType.VarChar:
                    retval = $"{attributeName} varchar({length})";
                    break;
                case DataType.Text:
                    retval = $"{attributeName} text";
                    break;
                case DataType.Boolean:
                    retval = $"{attributeName} bool";
                    break;
                case DataType.DateTime:
                    retval = $"{attributeName} timestamptz";
                    break;
                default:
                    retval = $"{attributeName} varchar({length})";
                    break;
            }
            return retval;
        }

        public static string ToOracleColumnString(DataType dataType, string attributeName, int length)
        {
            string retval;
            switch (dataType)
            {
                case DataType.Integer:
                    retval = $"{attributeName} number";
                    break;
                case DataType.Decimal:
                    retval = $"{attributeName} number(38, 2)";
                    break;
                case DataType.VarChar:
                    retval = $"{attributeName} nvarchar2({length})";
                    break;
                case DataType.Text:
                    retval = $"{attributeName} nclob";
                    break;
                case DataType.Boolean:
                    retval = $"{attributeName} number(1)";
                    break;
                case DataType.DateTime:
                    retval = $"{attributeName} timestamp(6) with time zone";
                    break;
                default:
                    retval = $"{attributeName} nvarchar2({length})";
                    break;
            }
            return retval;
        }

        public static string GetDateDiffLessThanYears(string fieldName, string years)
        {
            return GetDateDiffLessThan(fieldName, years, "YEAR");
        }

        public static string GetDateDiffLessThanMonths(string fieldName, string months)
        {
            return GetDateDiffLessThan(fieldName, months, "MONTH");
        }

        public static string GetDateDiffLessThanDays(string fieldName, string days)
        {
            return GetDateDiffLessThan(fieldName, days, "DAY");
        }

        public static string GetDateDiffLessThanHours(string fieldName, string hours)
        {
            return GetDateDiffLessThan(fieldName, hours, "HOUR");
        }

        public static string GetDateDiffLessThanMinutes(string fieldName, string minutes)
        {
            return GetDateDiffLessThan(fieldName, minutes, "MINUTE");
        }

        private static int GetSecondsByUnit(string unit)
        {
            var seconds = 1;
            if (unit == "MINUTE")
            {
                seconds = 60;
            }
            else if (unit == "HOUR")
            {
                seconds = 3600;
            }
            else if (unit == "DAY")
            {
                seconds = 86400;
            }
            else if (unit == "MONTH")
            {
                seconds = 2592000;
            }
            else if (unit == "YEAR")
            {
                seconds = 31536000;
            }
            return seconds;
        }

        private static string GetDateDiffLessThan(string fieldName, string fieldValue, string unit)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) < {fieldValue}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEDIFF({unit}, {fieldName}, getdate()) < {fieldValue}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} < {fieldValue}";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) < {fieldValue}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return retval;
        }

        public static string GetDateDiffGreatThanYears(string fieldName, string years)
        {
            return GetDateDiffGreatThan(fieldName, years, "YEAR");
        }

        public static string GetDateDiffGreatThanMonths(string fieldName, string months)
        {
            return GetDateDiffGreatThan(fieldName, months, "MONTH");
        }

        public static string GetDateDiffGreatThanDays(string fieldName, string days)
        {
            return GetDateDiffGreatThan(fieldName, days, "DAY");
        }

        public static string GetDateDiffGreatThanHours(string fieldName, string hours)
        {
            return GetDateDiffGreatThan(fieldName, hours, "HOUR");
        }

        public static string GetDateDiffGreatThanMinutes(string fieldName, string minutes)
        {
            return GetDateDiffGreatThan(fieldName, minutes, "MINUTE");
        }

        private static string GetDateDiffGreatThan(string fieldName, string fieldValue, string unit)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) > {fieldValue}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEDIFF({unit}, {fieldName}, getdate()) > {fieldValue}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} > {fieldValue}";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) > {fieldValue}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetDatePartYear(string fieldName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"DATE_FORMAT({fieldName}, '%Y')";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEPART([YEAR], {fieldName})";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"date_part('year', {fieldName})";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"EXTRACT(year from {fieldName})";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetDatePartMonth(string fieldName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"DATE_FORMAT({fieldName}, '%c')";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEPART([MONTH], {fieldName})";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"date_part('month', {fieldName})";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"EXTRACT(month from {fieldName})";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetDatePartDay(string fieldName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"DATE_FORMAT({fieldName}, '%e')";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEPART([DAY], {fieldName})";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"date_part('day', {fieldName})";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"EXTRACT(day from {fieldName})";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetDatePartHour(string fieldName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"DATE_FORMAT({fieldName}, '%k')";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEPART([HOUR], {fieldName})";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"date_part('hour', {fieldName})";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"EXTRACT(hour from {fieldName})";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetDatePartDayOfYear(string fieldName)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"DATE_FORMAT({fieldName}, '%j')";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"DATEPART([DAYOFYEAR], {fieldName})";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"date_part('doy', {fieldName})";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"TO_CHAR({fieldName}, 'DDD')";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetAddOne(string fieldName)
        {
            return GetAddNum(fieldName, 1);
        }

        public static string GetAddNum(string fieldName, int addNum)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"{fieldName} = IFNULL({fieldName}, 0) + {addNum}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"{fieldName} = ISNULL({fieldName}, 0) + {addNum}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"{fieldName} = COALESCE({fieldName}, 0) + {addNum}";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"{fieldName} = COALESCE({fieldName}, 0) + {addNum}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetMinusNum(string fieldName, int minusNum)
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = $"{fieldName} = IFNULL({fieldName}, 0) - {minusNum}";
                    break;
                case EDatabaseType.SqlServer:
                    retval = $"{fieldName} = ISNULL({fieldName}, 0) - {minusNum}";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
                    break;
                case EDatabaseType.Oracle:
                    retval = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static string GetOrderByRandom()
        {
            string retval;
            switch (WebConfigUtils.DatabaseType)
            {
                case EDatabaseType.MySql:
                    retval = "ORDER BY RAND()";
                    break;
                case EDatabaseType.SqlServer:
                    retval = "ORDER BY NEWID() DESC";
                    break;
                case EDatabaseType.PostgreSql:
                    retval = "ORDER BY random()";
                    break;
                case EDatabaseType.Oracle:
                    retval = "ORDER BY dbms_random.value()";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return retval;
        }

        public static int GetMaxLengthForNVarChar()
        {
            return 4000;
        }

        public static string ToSqlString(string inputString)
        {
            return !string.IsNullOrEmpty(inputString) ? inputString.Replace("'", "''") : string.Empty;
        }

        public static string ToSqlString(string inputString, int maxLength)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            if (maxLength > 0 && inputString.Length > maxLength)
            {
                inputString = inputString.Substring(0, maxLength);
            }
            return inputString.Replace("'", "''");
        }

        /// <summary>
        /// 验证此字符串是否合作作为字段名称
        /// </summary>
        public static bool IsAttributeNameCompliant(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName) || attributeName.IndexOf(" ", StringComparison.Ordinal) != -1) return false;
            if (-1 != attributeName.IndexOfAny(PathUtils.InvalidPathChars))
            {
                return false;
            }
            foreach (var t in attributeName)
            {
                if (StringUtils.IsTwoBytesChar(t))
                {
                    return false;
                }
            }
            return true;
        }

        public static string ReadNextSqlString(StreamReader reader)
        {
            try
            {
                var sb = new StringBuilder();

                while (true)
                {
                    var lineOfText = reader.ReadLine();

                    if (lineOfText == null)
                    {
                        return sb.Length > 0 ? sb.ToString() : null;
                    }

                    if (lineOfText.StartsWith("--")) continue;
                    lineOfText = lineOfText.Replace(")ENGINE=INNODB", ") ENGINE=InnoDB DEFAULT CHARSET=utf8");

                    if (lineOfText.TrimEnd().ToUpper() == "GO")
                    {
                        break;
                    }

                    sb.Append(lineOfText + Environment.NewLine);
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static string ReadNextStatementFromStream(StringReader reader)
        {
            try
            {
                var sb = new StringBuilder();

                while (true)
                {
                    var lineOfText = reader.ReadLine();
                    if (lineOfText == null)
                    {
                        return sb.Length > 0 ? sb.ToString() : null;
                    }

                    if (lineOfText.TrimEnd().ToUpper() == "GO")
                    {
                        break;
                    }

                    sb.Append(lineOfText + Environment.NewLine);
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static object Eval(object dataItem, string name)
        {
            object o = null;
            try
            {
                o = DataBinder.Eval(dataItem, name);
            }
            catch
            {
                // ignored
            }
            if (o == DBNull.Value)
            {
                o = null;
            }
            return o;
        }

        public static int EvalInt(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToInt32(o);
        }

        public static decimal EvalDecimal(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToDecimal(o);
        }

        public static string EvalString(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o?.ToString() ?? string.Empty;
        }

        public static DateTime EvalDateTime(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            if (o == null)
            {
                return DateUtils.SqlMinValue;
            }
            return (DateTime)o;
        }

        public static bool EvalBool(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o != null && TranslateUtils.ToBool(o.ToString());
        }

        public static string GetDatabaseNameFormConnectionString(string connectionString)
        {
            return GetValueFromConnectionString(connectionString, "database");
        }

        public static string GetValueFromConnectionString(string connectionString, string attribute)
        {
            //server=(local);uid=sa;pwd=bairong;Trusted_Connection=no;database=V1
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(attribute))
            {
                var pairs = connectionString.Split(';');
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        if (StringUtils.EqualsIgnoreCase(attribute, pair.Trim().Split('=')[0]))
                        {
                            retval = pair.Trim().Split('=')[1];
                            break;
                        }
                    }
                }
            }
            return retval;
        }

        
    }
}
