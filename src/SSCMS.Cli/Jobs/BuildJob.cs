using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class BuildJob : IJobService
    {
        public string CommandName => "build";

        private string _directory;
        private bool _isAll;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICreateManager _createManager;
        private readonly OptionSet _options;

        public BuildJob(ISettingsManager settingsManager, IDatabaseManager databaseManager, ICreateManager createManager)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _createManager = createManager;
            _options = new OptionSet
            {
                {
                    "d|directory=", "Site directory name",
                    v => _directory = v
                },
                {
                    "a|all", "Build all sites",
                    v => _isAll = v != null
                },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: generate static pages for the site");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/v7/cli/commands/build.html");
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

            var configPath = CliUtils.GetConfigPath(_settingsManager);
            if (!FileUtils.IsFileExists(configPath))
            {
                await console.WriteErrorAsync($"The sscms.json file does not exist: {configPath}");
                return;
            }

            await console.WriteLineAsync($"Database type: {_settingsManager.DatabaseType.GetDisplayName()}");
            await console.WriteLineAsync($"Database connection string: {_settingsManager.DatabaseConnectionString}");

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await console.WriteErrorAsync($"Unable to connect to database, error message: {errorMessage}");
                return;
            }

            if (_isAll)
            {
                var siteIds = await _databaseManager.SiteRepository.GetSiteIdsAsync();
                foreach (var siteId in siteIds)
                {
                    var site = await _databaseManager.SiteRepository.GetAsync(siteId);
                    await CreateSiteAsync(console, site);
                }
            }
            else
            {
                var directory = _directory;
                if (string.IsNullOrEmpty(directory))
                {
                    directory = string.Empty;
                }

                var site = await _databaseManager.SiteRepository.GetSiteByDirectoryAsync(directory);
                if (site == null)
                {
                    await console.WriteErrorAsync($"Unable to find the site, directory: {directory}");
                    return;
                }
                await CreateSiteAsync(console, site);
            }
        }

        private async Task CreateSiteAsync(ConsoleUtils console, Site site)
        {
            if (site != null)
            {
                await console.WriteLineAsync($"site: {site.SiteName}");
                await _createManager.ExecuteAsync(site.Id, CreateType.All, 0, 0, 0, 0);
                await console.WriteSuccessAsync("create pages successfully!");
            }
        }
    }
}
