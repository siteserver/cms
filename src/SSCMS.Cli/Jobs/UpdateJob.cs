using System;
using System.Linq;
using System.Threading.Tasks;
using Mono.Options;
using Semver;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class UpdateJob : IJobService
    {
        public string CommandName => "update";

        private bool? _isNightly;
        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IPathManager _pathManager;
        private readonly OptionSet _options;

        public UpdateJob(IApiService apiService, ISettingsManager settingsManager, IConfigRepository configRepository,
            IPathManager pathManager)
        {
            _apiService = apiService;
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _pathManager = pathManager;

            _options = new OptionSet
            {
                {
                    "nightly", "Update to nightly version",
                    v =>
                    {
                        if (string.IsNullOrEmpty(v))
                        {
                            _isNightly = null;
                        }
                        else
                        {
                            _isNightly = true;
                        }
                    }
                },
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: update sscms");
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

            var contentRootPath = _settingsManager.ContentRootPath;
            if (!CliUtils.IsSsCmsExists(contentRootPath) || await _configRepository.IsNeedInstallAsync())
            {
                await WriteUtils.PrintErrorAsync($"SS CMS has not been installed in {contentRootPath}");
                return;
            }

            var isNightly = _isNightly ?? _settingsManager.IsNightlyUpdate;

            var (success, result, failureMessage) = _apiService.GetReleases(isNightly, _settingsManager.Version, null);
            if (!success)
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
                return;
            }

            if (!SemVersion.TryParse(result.Cms.Version, out var version) || version <= _settingsManager.Version)
            {
                await WriteUtils.PrintErrorAsync("SS CMS is the latest version and no update is required");
                return;
            }

            var proceed = ReadUtils.GetYesNo($"New version {result.Cms.Version} found, do you want to update?");
            if (!proceed) return;

            Console.WriteLine($"Downloading {result.Cms.Version}...");
            CloudUtils.Dl.DownloadCms(_pathManager, result.Cms.Version);
            var name = CloudUtils.Dl.GetCmsDownloadName(result.Cms.Version);
            var packagePath = _pathManager.GetPackagesPath(name);

            var offlinePath = PathUtils.Combine(_settingsManager.ContentRootPath, "app_offline.htm");
            FileUtils.WriteText(offlinePath, "down for maintenance");

            foreach (var fileName in DirectoryUtils.GetFileNames(packagePath).Where(fileName =>
                !StringUtils.EqualsIgnoreCase(fileName, $"{name}.zip") &&
                !StringUtils.EqualsIgnoreCase(fileName, Constants.ConfigFileName)))
            {
                FileUtils.CopyFile(PathUtils.Combine(packagePath, fileName),
                    PathUtils.Combine(contentRootPath, fileName), true);
            }

            foreach (var directoryName in DirectoryUtils.GetDirectoryNames(packagePath))
            {
                DirectoryUtils.Copy(PathUtils.Combine(packagePath, directoryName), PathUtils.Combine(contentRootPath, directoryName), true);
            }

            FileUtils.DeleteFileIfExists(offlinePath);

            await WriteUtils.PrintSuccessAsync($"Congratulations, SS CMS was updated to {result.Cms.Version} successfully!");
        }
    }
}
