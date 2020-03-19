using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSCMS;
using SSCMS.Plugins;
using SSCMS.Cli.Core;
using SSCMS.Cli.Services;
using SSCMS.Cli.Updater;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Cli
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

                var executingAssembly = Assembly.GetExecutingAssembly();
                var assemblies = executingAssembly.GetReferencedAssemblies().Select(Assembly.Load).ToList();
                var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
                var fileAssemblies = Directory.GetFiles(path, $"{nameof(SSCMS)}*.dll").Select(Assembly.LoadFrom).ToArray();
                foreach (var referencedAssembly in fileAssemblies)
                {
                    if (!assemblies.Contains(referencedAssembly))
                    {
                        assemblies.Add(referencedAssembly);
                    }
                }
                if (!assemblies.Contains(executingAssembly))
                {
                    assemblies.Add(executingAssembly);
                }
                AssemblyUtils.SetAssemblies(assemblies);

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
