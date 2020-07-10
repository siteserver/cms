using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Datory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Core.Extensions;
using SSCMS.Core.Plugins.Extensions;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Tests
{
    public class IntegrationTestsFixture : IDisposable
    {
        public IConfiguration Configuration { get; }
        public string ContentRootPath { get; }
        public string WebRootPath { get; }
        public ServiceProvider Provider { get; }

        public IntegrationTestsFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            ContentRootPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(dirPath)));
            WebRootPath = PathUtils.Combine(ContentRootPath, Constants.WwwrootDirectory);

            Configuration = new ConfigurationBuilder()
                .SetBasePath(ContentRootPath)
                .AddJsonFile("sscms.json")
                .Build();

            var services = new ServiceCollection();

            ConfigureServices(services);

            Provider = services.BuildServiceProvider();

            var settingsManager = Provider.GetService<ISettingsManager>();
            if (settingsManager.Database.DatabaseType == DatabaseType.SQLite)
            {
                var filePath = PathUtils.Combine(settingsManager.ContentRootPath, Constants.DefaultLocalDbFileName);
                if (!FileUtils.IsFileExists(filePath))
                {
                    FileUtils.WriteText(filePath, string.Empty);
                }
            }
            var databaseManager = Provider.GetService<IDatabaseManager>();
            var pluginManager = Provider.GetService<IOldPluginManager>();
            databaseManager.SyncDatabaseAsync(pluginManager).GetAwaiter().GetResult();

            //var (_, repositories) = databaseRepository.GetAllRepositories(cache, settingsManager);

            //databaseRepository.InstallDatabaseAsync("admin", "admin888", repositories).GetAwaiter().GetResult();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var entryAssembly = Assembly.GetExecutingAssembly();
            var assemblies = new List<Assembly> { entryAssembly }.Concat(entryAssembly.GetReferencedAssemblies().Select(Assembly.Load));

            var settingsManager = services.AddSettingsManager(Configuration, ContentRootPath, WebRootPath, entryAssembly);
            services.AddPlugins(Configuration, settingsManager);

            services.AddCache(settingsManager.Redis.ConnectionString);

            services.AddRepositories(assemblies);
            services.AddServices();
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
