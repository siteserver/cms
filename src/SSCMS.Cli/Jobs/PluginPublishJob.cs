using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class PluginPublishJob : IJobService
    {
        public string CommandName => "plugin publish";

        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public PluginPublishJob(ISettingsManager settingsManager, IApiService apiService)
        {
            _settingsManager = settingsManager;
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
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: publish plugin to marketplace");
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

            var (success, _, failureMessage) = _apiService.GetStatus();
            if (!success)
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
                return;
            }

            var (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(_settingsManager.ContentRootPath);
            if (plugin == null)
            {
                await WriteUtils.PrintErrorAsync(errorMessage);
                return;
            }

            var packageId = PluginUtils.GetPackageId(plugin);
            var zipPath = PathUtils.Combine(_settingsManager.ContentRootPath, $"{packageId}.zip");
            if (!FileUtils.IsFileExists(zipPath))
            {
                PluginUtils.Package(plugin, _settingsManager.ContentRootPath);
            }
            var fileSize = FileUtils.GetFileSizeByFilePath(zipPath);

            await Console.Out.WriteLineAsync($"Packaged: {zipPath}");
            await Console.Out.WriteLineAsync($"Publishing {packageId} ({fileSize})...");

            (success, failureMessage) = _apiService.PluginsPublish(packageId, zipPath);
            if (success)
            {
                await WriteUtils.PrintSuccessAsync($"Published {packageId}, your plugin will live at https://www.sscms.com/plugins/{packageId} (might take a few minutes for it to show up).");
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
