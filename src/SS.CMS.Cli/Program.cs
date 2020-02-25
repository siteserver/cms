using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SS.CMS.Abstractions;
using SS.CMS.Cli.Core;
using SS.CMS.Cli.Services;
using SS.CMS.Cli.Updater;
using SS.CMS.Core;
using SS.CMS.Extensions;

namespace SS.CMS.Cli
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.GetEncoding(936);
            }
            catch
            {
                try
                {
                    Console.OutputEncoding = Encoding.UTF8;
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                var contentRootPath = Directory.GetCurrentDirectory();
                var builder = new ConfigurationBuilder()
                    .SetBasePath(contentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();

                var services = new ServiceCollection();
                var settingsManager = services.AddSettingsManager(configuration, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"));
                GlobalSettings.Load(settingsManager);
                services.AddCache(settingsManager.Redis.ConnectionString);
                services.AddRepositories();
                services.AddServices();

                services.AddTransient<Application>();
                services.AddTransient<BackupJob>();
                services.AddTransient<InstallJob>();
                services.AddTransient<RestoreJob>();
                services.AddTransient<TestJob>();
                services.AddTransient<UpdateJob>();
                services.AddTransient<VersionJob>();
                services.AddTransient<UpdaterManager>();

                var provider = services.BuildServiceProvider();
                CliUtils.Provider = provider;

                var application = provider.GetService<Application>();
                await application.RunAsync(args);
            }
            finally
            {
                Console.WriteLine("\r\nPress any key to exit...");
                Console.ReadKey();
            }
        }


    }
}
