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
                
                var application = new Application(settingsManager);
                services.AddSingleton(application);
                services.AddCache(settingsManager.Redis.ConnectionString);
                services.AddRepositories();
                services.AddServices();
                services.AddScoped<UpdaterManager>();

                services.AddScoped<IJobService, BackupJob>();
                services.AddScoped<IJobService, InstallJob>();
                services.AddScoped<IJobService, RestoreJob>();
                services.AddScoped<IJobService, SyncJob>();
                services.AddScoped<IJobService, TestJob>();
                services.AddScoped<IJobService, UpdateJob>();
                services.AddScoped<IJobService, VersionJob>();

                var provider = services.BuildServiceProvider();
                CliUtils.SetProvider(provider);

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
