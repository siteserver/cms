using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SSCMS.Cli.Core;
using SSCMS.Cli.Extensions;
using SSCMS.Core.Extensions;
using SSCMS.Core.Plugins.Extensions;
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

            var contentRootPath = Directory.GetCurrentDirectory();

            var profilePath = PathUtils.GetOsUserProfileDirectoryPath(Constants.OsUserProfileTypeConfig);
            var sscmsPath = PathUtils.Combine(contentRootPath, Constants.ConfigFileName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile(profilePath, optional: true, reloadOnChange: true)
                .AddJsonFile(sscmsPath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var services = new ServiceCollection().AddLogging(logging => logging.AddConsole());

            var entryAssembly = Assembly.GetExecutingAssembly();
            var assemblies = new List<Assembly> { entryAssembly }.Concat(entryAssembly.GetReferencedAssemblies().Select(Assembly.Load));

            var settingsManager = services.AddSettingsManager(configuration, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"), entryAssembly);
            await services.AddPluginsAsync(configuration, settingsManager);

            var application = new Application(settingsManager);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(application);
            services.AddCache(settingsManager.Redis.ConnectionString);

            services.AddRepositories(assemblies);
            services.AddServices();
            services.AddCliServices();
            services.AddCliJobs();

            var provider = services.BuildServiceProvider();
            CliUtils.SetProvider(provider);

            await application.RunAsync(args);

            //try
            //{

            //}
            //finally
            //{
            //    Console.WriteLine("\r\nPress any key to exit...");
            //    Console.ReadKey();
            //}
        }
    }
}
