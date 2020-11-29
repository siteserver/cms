using System;
using System.Threading.Tasks;
using Mono.Options;
using Semver;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class PluginPublishJob : IJobService
    {
        public string CommandName => "plugin publish";

        private string _version;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IApiService _apiService;
        private readonly OptionSet _options;

        public PluginPublishJob(ISettingsManager settingsManager, IPathManager pathManager, IApiService apiService)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _apiService = apiService;
            _options = new OptionSet
            {
                { "v|version=", "发布版本",
                    v => _version = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: publish plugin to marketplace");
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

            var (status, failureMessage) = _apiService.GetStatus();
            if (status == null)
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
                return;
            }

            var pluginId = string.Empty;
            if (context.Extras != null && context.Extras.Length > 0)
            {
                pluginId = context.Extras[0];
            }

            var pluginPath = string.IsNullOrEmpty(pluginId)
                ? _settingsManager.ContentRootPath
                : PathUtils.Combine(_pathManager.GetPluginPath(pluginId));

            var (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(pluginPath);
            if (plugin == null)
            {
                await WriteUtils.PrintErrorAsync(errorMessage);
                return;
            }

            if (!string.IsNullOrEmpty(_version))
            {
                SemVersion.TryParse(plugin.Version, out var pluginVersion);
                string versionChanged;
                
                if (_version == "major")
                {
                    versionChanged = pluginVersion.Change(pluginVersion.Major + 1).ToString();
                }
                else if (_version == "minor")
                {
                    versionChanged = pluginVersion.Change(pluginVersion.Major, pluginVersion.Minor + 1).ToString();
                }
                else if (_version == "patch")
                {
                    versionChanged = pluginVersion.Change(pluginVersion.Major, pluginVersion.Minor, pluginVersion.Patch + 1).ToString();
                }
                else if (PluginUtils.IsSemVersion(_version))
                {
                    versionChanged = _version;
                }
                else
                {
                    await WriteUtils.PrintErrorAsync($"Invalid plugin version '{_version}'");
                    return;
                }

                if (versionChanged != plugin.Version)
                {
                    await PluginUtils.UpdateVersionAsync(pluginPath, versionChanged);
                    (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(pluginPath);
                    if (plugin == null)
                    {
                        await WriteUtils.PrintErrorAsync(errorMessage);
                        return;
                    }
                }
            }

            var packageId = PluginUtils.GetPackageId(plugin.Publisher, plugin.Name, plugin.Version);
            var zipPath = PluginPackageJob.Package(_pathManager, plugin);
            var fileSize = FileUtils.GetFileSizeByFilePath(zipPath);

            await Console.Out.WriteLineAsync($"Packaged: {zipPath}");
            await Console.Out.WriteLineAsync($"Publishing {packageId} ({fileSize})...");

            bool success;
            (success, failureMessage) = _apiService.PluginPublish(plugin.Publisher, zipPath);
            if (success)
            {
                
                await WriteUtils.PrintSuccessAsync($"Published {packageId}, your plugin will live at {CloudUtils.Www.GetPluginUrl(plugin.Publisher, plugin.Name)}.");
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
