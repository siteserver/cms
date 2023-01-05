using System;
using System.Threading.Tasks;
using Mono.Options;
using Semver;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class UpdateJob : IJobService
    {
        public string CommandName => "update";

        private bool _isHelp;

        private readonly ICliApiService _cliApiService;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IPathManager _pathManager;
        private readonly OptionSet _options;

        public UpdateJob(ICliApiService cliApiService, ISettingsManager settingsManager, IConfigRepository configRepository,
            IPathManager pathManager)
        {
            _cliApiService = cliApiService;
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _pathManager = pathManager;

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
            await console.WriteLineAsync("Summary: update sscms");
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

            var contentRootPath = _settingsManager.ContentRootPath;
            if (!CliUtils.IsSsCmsExists(contentRootPath) || await _configRepository.IsNeedInstallAsync())
            {
                await console.WriteErrorAsync($"SS CMS has not been installed in {contentRootPath}");
                return;
            }

            var (success, result, failureMessage) = await _cliApiService.GetReleasesAsync(_settingsManager.Version, null);
            if (!success)
            {
                await console.WriteErrorAsync(failureMessage);
                return;
            }

            if (!SemVersion.TryParse(result.Cms.Version, out var version) || version <= _settingsManager.Version)
            {
                await console.WriteLineAsync($"SS CMS {result.Cms.Version} is the latest version and no update is required");
                var proceed = console.GetYesNo("do you still want to update?");
                if (!proceed) return;
            }
            else
            {
                var proceed = console.GetYesNo($"New version {result.Cms.Version} found, do you want to update?");
                if (!proceed) return;
            }

            await console.WriteLineAsync($"Downloading SS CMS {result.Cms.Version}...");
            var directoryPath = await CloudUtils.Dl.DownloadCmsAsync(_pathManager, _settingsManager.OSArchitecture, result.Cms.Version);

            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, Constants.ConfigFileName));
            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, "wwwroot/404.html"));
            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, "wwwroot/favicon.ico"));
            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, "wwwroot/index.html"));

            await console.WriteSuccessAsync($"{result.Cms.Version} download successfully!");
            await console.WriteLineAsync();
            await console.WriteLineAsync();

            await console.WriteLineAsync("Please stop website and override files and directories ");
            await console.WriteLineAsync($"     {directoryPath}");
            await console.WriteLineAsync("to");
            await console.WriteLineAsync($"     {contentRootPath}");

            var offlinePath = _pathManager.GetPackagesPath("app_offline.htm");
            FileUtils.WriteText(offlinePath, "down for maintenance");

            //var unOverrides = new List<string>();
            //foreach (var fileName in DirectoryUtils.GetFileNames(directoryPath))
            //{
            //    if (!FileUtils.CopyFile(PathUtils.Combine(directoryPath, fileName),
            //        PathUtils.Combine(contentRootPath, fileName), true))
            //    {
            //        unOverrides.Add(fileName);
            //    }
            //}

            //foreach (var directoryName in DirectoryUtils.GetDirectoryNames(directoryPath))
            //{
            //    DirectoryUtils.Copy(PathUtils.Combine(directoryPath, directoryName), PathUtils.Combine(contentRootPath, directoryName), true);
            //}

            //if (unOverrides.Count > 0)
            //{
            //    Replacing(contentRootPath, directoryPath, unOverrides);
            //}

            //FileUtils.DeleteFileIfExists(offlinePath);

            //await console.WriteSuccessAsync($"Congratulations, SS CMS was updated to {result.Cms.Version} successfully!");
        }

        //public static void Replacing(string contentRootPath, string directoryPath, List<string> unOverrides)
        //{
        //    Thread.Sleep(1000);
        //    var list = new List<string>();

        //    foreach (var unOverride in unOverrides)
        //    {
        //        await console.WriteLineAsync($"Replacing {unOverride}...");

        //        if (!FileUtils.CopyFile(PathUtils.Combine(directoryPath, unOverride),
        //            PathUtils.Combine(contentRootPath, unOverride), true))
        //        {
        //            list.Add(unOverride);
        //        }
        //    }

        //    if (list.Count > 0)
        //    {
        //        Replacing(contentRootPath, directoryPath, list);
        //    }
        //}
    }
}
