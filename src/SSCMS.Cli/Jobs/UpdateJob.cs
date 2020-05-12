using System;
using System.Threading.Tasks;
using Mono.Options;
using Semver;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Packaging;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class UpdateJob : IJobService
    {
        public string CommandName => "update";

        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IPathManager _pathManager;
        private readonly OptionSet _options;

        public UpdateJob(IApiService apiService, ISettingsManager settingsManager, IConfigRepository configRepository, IPathManager pathManager)
        {
            _apiService = apiService;
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _pathManager = pathManager;

            _options = new OptionSet {
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
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

            var (success, result, failureMessage) = _apiService.GetReleases(_settingsManager.IsNightlyUpdate, _settingsManager.Version, null);
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

            CloudUtils.DownloadCms(_pathManager, result.Cms.Version);
            var name = CloudUtils.GetCmsDownloadName(result.Cms.Version);
            var packagePath = _pathManager.GetPackagesPath(name);
            var packageZipPath = PathUtils.Combine(packagePath, $"{name}.zip");
            var packageConfigPath = PathUtils.Combine(packagePath, Constants.ConfigFileName);
            FileUtils.DeleteFileIfExists(packageZipPath);
            FileUtils.DeleteFileIfExists(packageConfigPath);

            DirectoryUtils.Copy(packagePath, contentRootPath, true);

            await WriteUtils.PrintSuccessAsync($"Congratulations, SS CMS was updated to {result.Cms.Version} successfully!");
        }
    }
}
