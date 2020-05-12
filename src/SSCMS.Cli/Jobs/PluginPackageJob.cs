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
    public class PluginPackageJob : IJobService
    {
        public string CommandName => "plugin package";

        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly OptionSet _options;

        public PluginPackageJob(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
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
            Console.WriteLine("Summary: package plugin to zip file");
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

            var (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(_settingsManager.ContentRootPath);
            if (plugin == null)
            {
                await WriteUtils.PrintErrorAsync(errorMessage);
                return;
            }

            PluginUtils.Package(plugin, _settingsManager.ContentRootPath);

            var packageId = PluginUtils.GetPackageId(plugin);
            var zipPath = PathUtils.Combine(_settingsManager.ContentRootPath, $"{packageId}.zip");
            var fileSize = FileUtils.GetFileSizeByFilePath(zipPath);

            await WriteUtils.PrintSuccessAsync($"Packaged: {zipPath} ({fileSize})");
        }
    }
}
