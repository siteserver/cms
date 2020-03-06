using System.Collections.Generic;
using Datory;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class InstallController
    {
        public class GetResult
        {
            public bool Forbidden { get; set; }
            public string ProductVersion { get; set; }

            public string NetVersion { get; set; }

            public string ContentRootPath { get; set; }
            public string WebRootPath { get; set; }

            public bool RootWritable { get; set; }

            public bool SiteFilesWritable { get; set; }

            public List<Select<string>> DatabaseTypes { get; set; }

            public string AdminUrl { get; set; }

            public List<Select<string>> OraclePrivileges { get; set; }
        }

        public class DatabaseConnectRequest
        {
            public DatabaseType DatabaseType { get; set; }
            public string DatabaseHost { get; set; }
            public bool IsDatabaseDefaultPort { get; set; }
            public string DatabasePort { get; set; }
            public string DatabaseUserName { get; set; }
            public string DatabasePassword { get; set; }
            public OraclePrivilege OraclePrivilege { get; set; }
            public bool OracleIsSid { get; set; }
            public string OracleDatabase { get; set; }
        }

        public class DatabaseConnectResult
        {
            public IList<string> DatabaseNames { get; set; }
        }

        public class RedisConnectRequest
        {
            public bool IsRedis { get; set; }
            public string RedisHost { get; set; }
            public bool IsRedisDefaultPort { get; set; }
            public int RedisPort { get; set; }
            public bool IsSsl { get; set; }
            public string RedisPassword { get; set; }
        }

        public class PrepareRequest : RedisConnectRequest
        {
            public DatabaseType DatabaseType { get; set; }
            public string DatabaseHost { get; set; }
            public bool IsDatabaseDefaultPort { get; set; }
            public string DatabasePort { get; set; }
            public string DatabaseUserName { get; set; }
            public string DatabasePassword { get; set; }
            public OraclePrivilege OraclePrivilege { get; set; }
            public bool OracleIsSid { get; set; }
            public string OracleDatabase { get; set; }
            public string DatabaseName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string AdminPassword { get; set; }
            public bool IsProtectData { get; set; }
        }

        public class InstallRequest : PrepareRequest
        {
            public string SecurityKey { get; set; }
        }

        private string GetDatabaseConnectionString(bool isDatabaseName, DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string selectedDatabaseName, string oracleDatabase, bool oracleIsSid, OraclePrivilege oraclePrivilege)
        {
            var databaseName = string.Empty;
            if (isDatabaseName)
            {
                databaseName = databaseType == DatabaseType.Oracle ? oracleDatabase : selectedDatabaseName;
            }

            var connectionString = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                connectionString = $"Server={server};";
                if (!isDefaultPort && port > 0)
                {
                    connectionString += $"Port={port};";
                }
                connectionString += $"Uid={userName};Pwd={password};";
                if (!string.IsNullOrEmpty(databaseName))
                {
                    connectionString += $"Database={databaseName};";
                }
                connectionString += "SslMode=Preferred;CharSet=utf8;";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                connectionString = $"Server={server};";
                if (!isDefaultPort && port > 0)
                {
                    connectionString = $"Server={server},{port};";
                }
                connectionString += $"Uid={userName};Pwd={password};";
                if (!string.IsNullOrEmpty(databaseName))
                {
                    connectionString += $"Database={databaseName};";
                }
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                connectionString = $"Host={server};";
                if (!isDefaultPort && port > 0)
                {
                    connectionString += $"Port={port};";
                }
                connectionString += $"Username={userName};Password={password};";
                if (!string.IsNullOrEmpty(databaseName))
                {
                    connectionString += $"Database={databaseName};";
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                databaseName = oracleIsSid ? $"SID={databaseName}" : $"SERVICE_NAME={databaseName}";
                port = !isDefaultPort && port > 0 ? port : 1521;
                var privilegeString = string.Empty;
                if (oraclePrivilege == OraclePrivilege.SYSDBA)
                {
                    privilegeString = "DBA Privilege=SYSDBA;";
                }
                else if (oraclePrivilege == OraclePrivilege.SYSDBA)
                {
                    privilegeString = "DBA Privilege=SYSOPER;";
                }
                databaseName = string.IsNullOrEmpty(databaseName)
                    ? string.Empty
                    : $"(CONNECT_DATA=({databaseName}))";
                connectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port})){databaseName});User ID={userName};Password={password};{privilegeString}";
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                connectionString = "Data Source=~/ss.sqlite;Version=3;";
            }

            return connectionString;
        }

        private string GetRedisConnectionString(RedisConnectRequest request)
        {
            var connectionString = string.Empty;
            if (request.IsRedis && !string.IsNullOrEmpty(request.RedisHost))
            {
                var port = 6379;
                if (!request.IsRedisDefaultPort && request.RedisPort > 0)
                {
                    port = request.RedisPort;
                }
                connectionString = $"{request.RedisHost}:{port},allowAdmin=true";
                if (request.IsSsl)
                {
                    connectionString += ",ssl=true";
                }
                if (!string.IsNullOrEmpty(request.RedisPassword))
                {
                    connectionString += $",password={request.RedisPassword}";
                }
            }

            return connectionString;
        }
    }
}