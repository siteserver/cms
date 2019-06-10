using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SS.CMS.Utils;

namespace SS.CMS.Data.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public IDb Db { get; }

        public EnvironmentFixture()
        {
            var projDirectoryPath = DirectoryUtils.GetParentPath(Directory.GetCurrentDirectory(), 3);

            var config = new ConfigurationBuilder()
                .SetBasePath(projDirectoryPath)
                .AddJsonFile("appSettings.json")
                .Build();
            var databaseType = DatabaseType.GetDatabaseType(config["ss:database:type"]);
            var connectionString = config["ss:database:connectionString"];

            Db = new Db(databaseType, connectionString);
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
