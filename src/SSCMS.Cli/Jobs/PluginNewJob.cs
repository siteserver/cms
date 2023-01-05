using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Configuration;
using SSCMS.Core.Plugins;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class PluginNewJob : IJobService
    {
        public string CommandName => "plugin new";

        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ICliApiService _cliApiService;
        private readonly OptionSet _options;

        public PluginNewJob(ISettingsManager settingsManager, IPathManager pathManager, ICliApiService cliApiService)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _cliApiService = cliApiService;
            _options = new OptionSet
            {
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: creates a new plugin, includes configuration based on the specified parameters.");
            await console.WriteLineAsync("Options:");
            _options.WriteOptionDescriptions(console.Out);
            await console.WriteLineAsync();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            using var console = new ConsoleUtils(false);
            if (_isHelp)
            {
                await WriteUsageAsync(console);
                return;
            }

            var pluginsPath = CliUtils.IsSsCmsExists(_settingsManager.ContentRootPath)
                ? _pathManager.PluginsPath
                : _settingsManager.ContentRootPath;

            var (status, _) = await _cliApiService.GetStatusAsync();
            var publisher = status == null
                ? console.GetString("What's the publisher of your plugin?")
                : status.UserName;

            if (status == null && !StringUtils.IsStrictName(publisher))
            {
                await console.WriteErrorAsync(
                    $@"Invalid plugin publisher: ""{publisher}"", string does not match the pattern of ""{StringUtils.StrictNameRegex}""");
                return;
            }

            var name = console.GetString("What's the name of your plugin?");
            if (!StringUtils.IsStrictName(name))
            {
                await console.WriteErrorAsync(
                    $@"Invalid plugin name: ""{publisher}"", string does not match the pattern of ""{StringUtils.StrictNameRegex}""");
                return;
            }

            var pluginId = PluginUtils.GetPluginId(publisher, name);
            var pluginPath = PathUtils.Combine(pluginsPath, pluginId);

            var dict = new Dictionary<string, object>
            {
                ["name"] = name,
                ["publisher"] = publisher
            };
            var json = TranslateUtils.JsonSerialize(dict);
            await FileUtils.WriteTextAsync(PathUtils.Combine(pluginPath, Constants.PackageFileName), json);

            await console.WriteSuccessAsync($@"The plugin ""{pluginId}"" was created successfully.");
        }
    }
}
