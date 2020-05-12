using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Packaging;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class InstallJob : IJobService
    {
        public string CommandName => "install";

        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly OptionSet _options;

        public InstallJob(IApiService apiService, ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository)
        {
            _apiService = apiService;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;

            _options = new OptionSet {
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: install sscms");
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

            InstallUtils.Init(contentRootPath);

            if (!await _configRepository.IsNeedInstallAsync())
            {
                await WriteUtils.PrintErrorAsync($"SS CMS has been installed in {contentRootPath}");
                return;
            }

            var proceed = ReadUtils.GetYesNo($"Do you want to install SS CMS in {contentRootPath}?");
            if (!proceed) return;

            if (!CliUtils.IsSsCmsExists(contentRootPath))
            {
                var (success, result, errorMessage) = _apiService.GetReleases(false, string.Empty, null);
                if (!success)
                {
                    await WriteUtils.PrintErrorAsync(errorMessage);
                    return;
                }

                CloudUtils.DownloadCms(_pathManager, result.Cms.Version, true);
            }

            var databaseTypeInput = ReadUtils.GetSelect("Database type", new List<string>
            {
                DatabaseType.MySql.GetValue().ToLower(),
                DatabaseType.SqlServer.GetValue().ToLower(),
                DatabaseType.PostgreSql.GetValue().ToLower(),
                DatabaseType.SQLite.GetValue().ToLower()
            });

            var databaseType = TranslateUtils.ToEnum(databaseTypeInput, DatabaseType.MySql);
            var databaseName = string.Empty;
            var databaseHost = string.Empty;
            var isDatabaseDefaultPort = true;
            var databasePort = 0;
            var databaseUserName = string.Empty;
            var databasePassword = string.Empty;

            if (databaseType != DatabaseType.SQLite)
            {
                databaseHost = ReadUtils.GetString("Database hostname / IP:");
                isDatabaseDefaultPort = ReadUtils.GetYesNo("Use default port?");
                
                if (!isDatabaseDefaultPort)
                {
                    databasePort = ReadUtils.GetInt("Database port:");
                }
                databaseUserName = ReadUtils.GetString("Database userName:");
                databasePassword = ReadUtils.GetPassword("Database password:");

                var connectionStringWithoutDatabaseName = InstallUtils.GetDatabaseConnectionString(databaseType, databaseHost, isDatabaseDefaultPort, databasePort, databaseUserName, databasePassword, string.Empty);

                var db = new Database(databaseType, connectionStringWithoutDatabaseName);

                var (success, errorMessage) = await db.IsConnectionWorksAsync();
                if (!success)
                {
                    await WriteUtils.PrintErrorAsync(errorMessage);
                    return;
                }

                var databaseNames = await db.GetDatabaseNamesAsync();
                databaseName = ReadUtils.GetSelect("Database name", databaseNames);
            }

            var userName = ReadUtils.GetString("Super administrator username:");
            var password = ReadUtils.GetPassword("Super administrator password:");

            var (valid, message) =
                await _administratorRepository.InsertValidateAsync(userName, password, string.Empty, string.Empty);
            if (!valid)
            {
                await WriteUtils.PrintErrorAsync(message);
                return;
            }

            if (databaseType == DatabaseType.SQLite)
            {
                var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.DefaultLocalDbFileName);
                if (!FileUtils.IsFileExists(filePath))
                {
                    await FileUtils.WriteTextAsync(filePath, string.Empty);
                }
            }

            var databaseConnectionString = InstallUtils.GetDatabaseConnectionString(databaseType, databaseHost, isDatabaseDefaultPort, databasePort, databaseUserName, databasePassword, databaseName);

            var isProtectData = ReadUtils.GetYesNo("Protect settings?");
            await _settingsManager.SaveSettingsAsync(false, isProtectData, databaseType, databaseConnectionString, string.Empty);

            (valid, message) = await _databaseManager.InstallAsync(_pluginManager, userName, password, string.Empty, string.Empty);
            if (!valid)
            {
                await WriteUtils.PrintErrorAsync(message);
                return;
            }

            await FileUtils.WriteTextAsync(_pathManager.GetWebRootPath("index.html"), Constants.Html5Empty);

            await WriteUtils.PrintSuccessAsync("Congratulations, SS CMS was installed successfully!");
        }
    }
}
