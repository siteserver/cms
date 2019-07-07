using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Cms
{
    public partial class CmsController
    {
        private const string RouteInstallTryDatabase = "install/tryDatabase";
        private const string RouteInstallTryCache = "install/tryCache";
        private const string RouteInstallSaveSettings = "install/saveSettings";
        private const string RouteInstall = "install";

        public class TryDatabaseRequest
        {
            public string DatabaseType { get; set; }
            public string Server { get; set; }
            public int? Port { get; set; }
            public string Uid { get; set; }
            public string Pwd { get; set; }
            public string ConnectionString { get; set; }
        }

        public class TryDatabaseResponse
        {
            public bool IsSuccess { get; set; }
            public IList<string> DatabaseNames { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class TryCacheRequest
        {
            public string CacheType { get; set; }
            public string Server { get; set; }
            public int? Port { get; set; }
            public string Uid { get; set; }
            public string Pwd { get; set; }
            public string ConnectionString { get; set; }
        }

        public class TryCacheResponse
        {
            public bool IsSuccess { get; set; }
            public IList<string> DatabaseNames { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class SaveSettingsRequest
        {
            public string DatabaseType { get; set; }
            public string DatabaseServer { get; set; }
            public int? DatabasePort { get; set; }
            public string DatabaseUid { get; set; }
            public string DatabasePwd { get; set; }
            public string DatabaseName { get; set; }
            public string DatabaseConnectionString { get; set; }
            public string CacheType { get; set; }
            public string CacheServer { get; set; }
            public int? CachePort { get; set; }
            public string CacheUid { get; set; }
            public string CachePwd { get; set; }
            public string CacheName { get; set; }
            public string CacheConnectionString { get; set; }
            public bool IsProtectData { get; set; }
        }

        public class SaveSettingsResponse
        {
            public string SecurityKey { get; set; }
        }

        public class InstallRequest
        {
            public string SecurityKey { get; set; }
            public string AdminName { get; set; }
            public string AdminPassword { get; set; }
        }

        public class InstallResponse
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
        }

        private string GetDatabaseConnectionString(DatabaseType databaseType, string server, int? port, string uid, string pwd, string databaseName = null)
        {
            if (databaseType == DatabaseType.SQLite)
            {
                return "Data Source=~/ss.sqlite;Version=3;";
            }

            var connectionString = $"Server={server};";
            if (port.HasValue && port.Value > 0)
            {
                connectionString += $"Port={port.Value};";
            }
            connectionString += $"Uid={uid};Pwd={pwd};";
            if (!string.IsNullOrEmpty(databaseName))
            {
                connectionString += $"Database={databaseName};";
            }
            return connectionString;
        }

        private string GetRedisConnectionString(string server, int? port, string pwd)
        {
            var connectionString = server;
            if (port.HasValue && port.Value > 0)
            {
                connectionString += $":{port.Value}";
            }
            if (!string.IsNullOrEmpty(pwd))
            {
                connectionString += $",password={pwd}";
            }
            return connectionString;
        }
    }
}