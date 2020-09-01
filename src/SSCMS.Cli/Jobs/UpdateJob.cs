using System;
using System.Threading.Tasks;
using Mono.Options;
using Semver;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Configuration;
using SSCMS.Core.Plugins;
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
                    "h|help", "Display help",
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

        public async Task ExecuteAsync(IPluginJobContext context)
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

            var (success, result, failureMessage) = _apiService.GetReleases(_settingsManager.Version, null);
            if (!success)
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
                return;
            }

            if (!SemVersion.TryParse(result.Cms.Version, out var version) || version <= _settingsManager.Version)
            {
                Console.WriteLine($"SS CMS {result.Cms.Version} is the latest version and no update is required");
                var proceed = ReadUtils.GetYesNo("do you still want to update?");
                if (!proceed) return;
            }
            else
            {
                var proceed = ReadUtils.GetYesNo($"New version {result.Cms.Version} found, do you want to update?");
                if (!proceed) return;
            }

            Console.WriteLine($"Downloading SS CMS {result.Cms.Version}...");
            var directoryPath = CloudUtils.Dl.DownloadCms(_pathManager, _settingsManager.OSArchitecture, result.Cms.Version);

            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, Constants.ConfigFileName));
            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, "wwwroot/404.html"));
            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, "wwwroot/favicon.ico"));
            FileUtils.DeleteFileIfExists(PathUtils.Combine(directoryPath, "wwwroot/index.html"));

            await WriteUtils.PrintSuccessAsync($"{result.Cms.Version} download successfully!");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Please stop website and override files and directories ");
            Console.WriteLine($"     {directoryPath}");
            Console.WriteLine("to");
            Console.WriteLine($"     {contentRootPath}");

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

            //await WriteUtils.PrintSuccessAsync($"Congratulations, SS CMS was updated to {result.Cms.Version} successfully!");
        }

        //public static void Replacing(string contentRootPath, string directoryPath, List<string> unOverrides)
        //{
        //    Thread.Sleep(1000);
        //    var list = new List<string>();

        //    foreach (var unOverride in unOverrides)
        //    {
        //        Console.WriteLine($"Replacing {unOverride}...");

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
