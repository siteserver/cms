using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
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
        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public PluginNewJob(ISettingsManager settingsManager, IPathManager pathManager, IApiService apiService)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _apiService = apiService;
            _options = new OptionSet
            {
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: creates a new plugin, includes configuration based on the specified parameters.");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var pluginsPath = CliUtils.IsSsCmsExists(_settingsManager.ContentRootPath)
                ? _pathManager.PluginsPath
                : _settingsManager.ContentRootPath;

            var (status, _) = _apiService.GetStatus();
            var publisher = status == null
                ? ReadUtils.GetString("What's the publisher of your plugin?")
                : status.UserName;

            if (status == null && !StringUtils.IsStrictName(publisher))
            {
                await WriteUtils.PrintErrorAsync(
                    $@"Invalid plugin publisher: ""{publisher}"", string does not match the pattern of ""{StringUtils.StrictNameRegex}""");
                return;
            }

            var name = ReadUtils.GetString("What's the name of your plugin?");
            if (!StringUtils.IsStrictName(name))
            {
                await WriteUtils.PrintErrorAsync(
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

            await WriteUtils.PrintSuccessAsync($@"The plugin ""{pluginId}"" was created successfully.");
        }
    }
}
