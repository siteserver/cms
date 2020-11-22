using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class ThemePublishJob : IJobService
    {
        public string CommandName => "theme publish";

        private string _directory;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly IPathManager _pathManager;
        private readonly ICacheManager _cacheManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IApiService _apiService;

        public ThemePublishJob(IPathManager pathManager, ICacheManager cacheManager, IDatabaseManager databaseManager, IApiService apiService)
        {
            _options = new OptionSet
            {
                { "d|directory=", "site directory path",
                    v => _directory = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };

            _pathManager = pathManager;
            _cacheManager = cacheManager;
            _databaseManager = databaseManager;
            _apiService = apiService;
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: publish theme to marketplace");
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

            var (success, name, filePath) = await ThemePackageJob.PackageAsync(_pathManager, _cacheManager, _databaseManager,
                _directory, false);

            if (!success) return;

            var fileSize = FileUtils.GetFileSizeByFilePath(filePath);

            await Console.Out.WriteLineAsync($"Theme Packaged: {filePath}");
            await Console.Out.WriteLineAsync($"Publishing theme {name} ({fileSize})...");

            (success, failureMessage) = _apiService.ThemePublish(filePath);
            if (success)
            {
                await WriteUtils.PrintSuccessAsync($"Theme published, your theme will live at {CloudUtils.Www.GetThemeUrl(status.UserName, name)}.");
            }
            else
            {
                await WriteUtils.PrintErrorAsync(failureMessage);
            }
        }
    }
}
