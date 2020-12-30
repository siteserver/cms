using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Cli.Core;
using SSCMS.Cli.Extensions;
using SSCMS.Core.Extensions;
using SSCMS.Core.Plugins.Extensions;
using SSCMS.Utils;
using Serilog;
using SSCMS.Cli.Abstractions;
using SSCMS.Configuration;

namespace SSCMS.Cli
{
    public static class Program
    {
        public static IApplication Application { get; private set; }

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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var contentRootPath = Directory.GetCurrentDirectory();

            var profilePath = CliUtils.GetOsUserConfigFilePath();
            var sscmsPath = PathUtils.Combine(contentRootPath, Constants.ConfigFileName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile(profilePath, optional: true, reloadOnChange: true)
                .AddJsonFile(sscmsPath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/cli/log.log", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();

            var services = new ServiceCollection();

            var entryAssembly = Assembly.GetExecutingAssembly();
            var assemblies = new List<Assembly> { entryAssembly }.Concat(entryAssembly.GetReferencedAssemblies().Select(Assembly.Load));

            var settingsManager = services.AddSettingsManager(configuration, contentRootPath, PathUtils.Combine(contentRootPath, Constants.WwwrootDirectory), entryAssembly);
            services.AddPlugins(configuration, settingsManager);
            //services.AddPluginServices(pluginManager);

            Application = new Application(settingsManager);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddCache(settingsManager.Redis.ConnectionString);

            services.AddRepositories(assemblies);
            services.AddServices();
            services.AddCliServices();
            services.AddCliJobs();

            await Application.RunAsync(args);
        }
    }
}
