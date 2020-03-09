using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SS.CMS.Abstractions;
using SS.CMS.Extensions;

namespace SS.CMS.Tests
{
    public class IntegrationTestsFixture : IDisposable
    {
        public IConfiguration Configuration { get; }
        public ServiceProvider Provider { get; set; }
        public ISettingsManager SettingsManager { get; set; }

        public IntegrationTestsFixture()
        {
            var contentRootPath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("ss.json")
                .Build();

            var services = new ServiceCollection();
            SettingsManager = services.AddSettingsManager(configuration, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"));
            services.AddCache(SettingsManager.Redis.ConnectionString);
            services.AddRepositories();
            services.AddServices();
            Provider = services.BuildServiceProvider();
            Configuration = configuration;


            //var tableNames = settingsManager.Database.GetTableNamesAsync().GetAwaiter().GetResult();
            //foreach (var tableName in tableNames)
            //{
            //    settingsManager.Database.DropTableAsync(tableName).GetAwaiter().GetResult();
            //}

            //var databaseRepository = provider.GetService<IDatabaseRepository>();

            //var (_, repositories) = databaseRepository.GetAllRepositories(cache, settingsManager);

            //databaseRepository.InstallDatabaseAsync("admin", "admin888", repositories).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            //var db = new Database(SettingsManager.DatabaseType, SettingsManager.DatabaseConnectionString);
            //var tableNames = db.GetTableNamesAsync().GetAwaiter().GetResult();
            //foreach (var tableName in tableNames)
            //{
            //    db.DropTableAsync(tableName).GetAwaiter().GetResult();
            //}
            // ... clean up test data from the database ...
        }
    }
}
