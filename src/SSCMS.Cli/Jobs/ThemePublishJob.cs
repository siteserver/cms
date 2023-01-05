using System;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Utils;
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
        private readonly ICliApiService _cliApiService;

        public ThemePublishJob(IPathManager pathManager, ICacheManager cacheManager, IDatabaseManager databaseManager, ICliApiService cliApiService)
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
            _cliApiService = cliApiService;
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: publish theme to marketplace");
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

            var (status, failureMessage) = await _cliApiService.GetStatusAsync();
            if (status == null)
            {
                await console.WriteErrorAsync(failureMessage);
                return;
            }

            var (success, name, filePath) = await ThemePackageJob.PackageAsync(console, _pathManager, _cacheManager, _databaseManager,
                _directory, false);

            if (!success) return;

            var fileSize = FileUtils.GetFileSizeByFilePath(filePath);

            await console.WriteLineAsync($"Theme Packaged: {filePath}");
            await console.WriteLineAsync($"Publishing theme {name} ({fileSize})...");

            (success, failureMessage) = await _cliApiService.ThemePublishAsync(filePath);
            if (success)
            {
                await console.WriteSuccessAsync($"Theme published, your theme will live at {CloudUtils.Www.GetThemeUrl(status.UserName, name)}.");
            }
            else
            {
                await console.WriteErrorAsync(failureMessage);
            }
        }
    }
}
