using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.Settings;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public IDb Db { get; }
        public AppSettings AppSettings { get; }

        public IMemoryCache MemoryCache { get; }

        public EnvironmentFixture()
        {
            var projDirectoryPath = DirectoryUtils.GetParentPath(Directory.GetCurrentDirectory(), 3);

            var config = new ConfigurationBuilder()
                .SetBasePath(projDirectoryPath)
                .AddJsonFile("appSettings.json")
                .Build();

            AppSettings = new AppSettings
            {
                Database = new DatabaseSettings
                {
                    Type = config["ss:database:type"],
                    ConnectionString = config["ss:database:connectionString"]
                },
                SecretKey = config["ss:secretKey"]
            };

            Db = new Db(DatabaseType.GetDatabaseType(AppSettings.Database.Type), AppSettings.Database.ConnectionString);
            MemoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
