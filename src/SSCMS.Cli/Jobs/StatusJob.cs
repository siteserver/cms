using System;
using System.Reflection;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class StatusJob : IJobService
    {
        public string CommandName => "status";

        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public StatusJob(ISettingsManager settingsManager, IPluginManager pluginManager, IApiService apiService)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _apiService = apiService;
            _options = new OptionSet
            {
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: show user login status");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            await Console.Out.WriteLineAsync($"Cli version: {_settingsManager.Version}");
            var entryAssembly = Assembly.GetExecutingAssembly();
            await Console.Out.WriteLineAsync($"Cli location: {entryAssembly.Location}");
            await Console.Out.WriteLineAsync($"Work location: {_settingsManager.ContentRootPath}");
            await Console.Out.WriteLineAsync($"Api host: {CloudUtils.CloudApiHost}");

            var configPath = CliUtils.GetConfigPath(_settingsManager);
            if (FileUtils.IsFileExists(configPath))
            {
                await Console.Out.WriteLineAsync($"Database type: {_settingsManager.Database.DatabaseType.GetDisplayName()}");
                await Console.Out.WriteLineAsync($"Database connection string: {_settingsManager.DatabaseConnectionString}");

                if (!string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString))
                {
                    var (isConnectionWorks, errorMessage) =
                        await _settingsManager.Database.IsConnectionWorksAsync();

                    if (!isConnectionWorks)
                    {
                        await WriteUtils.PrintErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                        return;
                    }

                    await Console.Out.WriteLineAsync("Database status: Connection successful");
                }

                var plugins = _pluginManager.Plugins;
                foreach (var plugin in plugins)
                {
                    await Console.Out.WriteLineAsync($"PluginId: {plugin.PluginId}, Version: {plugin.Version}");
                }
            }
            else
            {
                await Console.Out.WriteLineAsync($"The sscms.json file does not exist: {configPath}");
            }

            var (status, _) = _apiService.GetStatus();
            if (status != null)
            {
                await Console.Out.WriteLineAsync($"Login user: {status.UserName}");
            }
        }
    }
}