using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class InstallController
    {
        public class Environment
        {
            public bool IsInstalled { get; set; }
            public string ApiUrl { get; set; }
            public string ProductVersion { get; set; }
            public string DotNetVersion { get; set; }
            public string ContentRootPath { get; set; }
            public string WebRootPath { get; set; }
            public bool IsContentRootPathWritable { get; set; }
            public bool IsWebRootPathWritable { get; set; }
            public string TargetFramework { get; set; }
        }

        public class DatabaseModel
        {
            public string DatabaseType { get; set; }
            public string Server { get; set; }
            public int? Port { get; set; }
            public string Uid { get; set; }
            public string Pwd { get; set; }
            public string DatabaseName { get; set; }
            public bool isRedisEnabled { get; set; }
        }

        public class RedisModel
        {
            public string RedisConnectionString { get; set; }
        }


        public class SettingsModel : DatabaseModel
        {
            public string AdminUrl { get; set; }
            public string HomeUrl { get; set; }
            public string Language { get; set; }
            public bool IsProtectData { get; set; }
            public bool RedisIsEnabled { get; set; }
            public string RedisConnectionString { get; set; }
            public string AdminName { get; set; }
            public string AdminPassword { get; set; }
        }

        public class ResultModel
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
        }

        private async Task<string> GetConnectionAsync(DatabaseType databaseType, DatabaseModel database)
        {
            if (databaseType == DatabaseType.SQLite)
            {
                var dbFilePath = PathUtils.Combine(_settingsManager.ContentRootPath, "database.sqlite");
                if (!FileUtils.IsFileExists(dbFilePath))
                {
                    await FileUtils.WriteTextAsync(PathUtils.Combine(_settingsManager.ContentRootPath, "database.sqlite"), string.Empty);
                }

                return "Data Source=~/database.sqlite;Version=3;";
            }

            var connectionString = $"Server={database.Server};";
            if (!database.Port.HasValue && database.Port > 0)
            {
                connectionString += $"Port={database.Port};";
            }
            connectionString += $"Uid={database.Uid};Pwd={database.Pwd};";
            if (!string.IsNullOrEmpty(database.DatabaseName))
            {
                connectionString += $"Database={database.DatabaseName};";
            }
            return connectionString;
        }
    }
}