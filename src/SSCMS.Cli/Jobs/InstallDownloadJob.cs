using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class InstallDownloadJob : IJobService
    {
        public string CommandName => "install download";

        private bool _isHelp;

        private readonly IApiService _apiService;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly OptionSet _options;

        public InstallDownloadJob(IApiService apiService, ISettingsManager settingsManager, IPathManager pathManager, IConfigRepository configRepository)
        {
            _apiService = apiService;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _configRepository = configRepository;

            _options = new OptionSet {
                { "h|help",  "Display help",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: download sscms and save settings");
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

            if (!CliUtils.IsSsCmsExists(contentRootPath))
            {
                var (success, result, failureMessage) = _apiService.GetReleases(_settingsManager.Version, null);
                if (!success)
                {
                    await WriteUtils.PrintErrorAsync(failureMessage);
                    return;
                }

                var proceed = ReadUtils.GetYesNo($"Do you want to install SS CMS in {contentRootPath}?");
                if (!proceed) return;

                Console.WriteLine($"Downloading SS CMS {result.Cms.Version}...");
                var directoryPath = CloudUtils.Dl.DownloadCms(_pathManager, _settingsManager.OSArchitecture, result.Cms.Version);

                await WriteUtils.PrintSuccessAsync($"{result.Cms.Version} download successfully!");

                DirectoryUtils.Copy(directoryPath, contentRootPath, true);
            }

            InstallUtils.Init(contentRootPath);

            if (!await _configRepository.IsNeedInstallAsync())
            {
                await WriteUtils.PrintErrorAsync($"SS CMS has been installed in {contentRootPath}");
                return;
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

            var databaseConnectionString = InstallUtils.GetDatabaseConnectionString(databaseType, databaseHost, isDatabaseDefaultPort, databasePort, databaseUserName, databasePassword, databaseName);

            var isProtectData = ReadUtils.GetYesNo("Protect settings in sscms.json?");
            _settingsManager.SaveSettings(isProtectData, false, databaseType, databaseConnectionString, string.Empty, string.Empty, null, null);

            await WriteUtils.PrintSuccessAsync("SS CMS was download and ready for install, please run: sscms install database");

        }
    }
}
