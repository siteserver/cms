﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using Mono.Options;
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
    public class InstallDownloadJob : IJobService
    {
        public string CommandName => "install download";

        private bool _isHelp;

        private readonly ICliApiService _cliApiService;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly OptionSet _options;

        public InstallDownloadJob(ICliApiService cliApiService, ISettingsManager settingsManager, IPathManager pathManager, IConfigRepository configRepository)
        {
            _cliApiService = cliApiService;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _configRepository = configRepository;

            _options = new OptionSet {
                { "h|help",  "Display help",
                    v => _isHelp = v != null }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: download sscms and save settings");
            await console.WriteLineAsync($"Docs: {Constants.OfficialHost}/docs/v7/cli/commands/install-download.html");
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

            if (!CliUtils.IsSsCmsExists(contentRootPath))
            {
                var (success, result, failureMessage) = await _cliApiService.GetReleasesAsync(_settingsManager.Version, null);
                if (!success)
                {
                    await console.WriteErrorAsync(failureMessage);
                    return;
                }

                var proceed = console.GetYesNo($"Do you want to install SSCMS in {contentRootPath}?");
                if (!proceed) return;

                await console.WriteLineAsync($"Downloading SSCMS {result.Cms.Version}...");
                var directoryPath = await CloudUtils.Dl.DownloadCmsAsync(_pathManager, _settingsManager.OSArchitecture, result.Cms.Version);

                await console.WriteSuccessAsync($"{result.Cms.Version} download successfully!");

                DirectoryUtils.Copy(directoryPath, contentRootPath, true);
            }

            InstallUtils.Init(contentRootPath);

            if (!await _configRepository.IsNeedInstallAsync())
            {
                await console.WriteErrorAsync($"SSCMS has been installed in {contentRootPath}");
                return;
            }

            var databaseValues = ListUtils.GetEnums<DatabaseType>().Select(x => x.GetValue().ToLower()).ToList();

            var databaseTypeInput = console.GetSelect("Database type", databaseValues);

            var databaseType = TranslateUtils.ToEnum(databaseTypeInput, DatabaseType.MySql);
            var databaseName = string.Empty;
            var databaseHost = string.Empty;
            var isDatabaseDefaultPort = true;
            var databasePort = 0;
            var databaseUserName = string.Empty;
            var databasePassword = string.Empty;

            if (databaseType != DatabaseType.SQLite)
            {
                databaseHost = console.GetString("Database hostname / IP:");
                isDatabaseDefaultPort = console.GetYesNo("Use default port?");
                
                if (!isDatabaseDefaultPort)
                {
                    databasePort = console.GetInt("Database port:");
                }
                databaseUserName = console.GetString("Database userName:");
                databasePassword = console.GetPassword("Database password:");

                var connectionStringWithoutDatabaseName = DbUtils.GetConnectionString(databaseType, databaseHost, isDatabaseDefaultPort, databasePort, databaseUserName, databasePassword, string.Empty);

                var db = new Database(databaseType, connectionStringWithoutDatabaseName);

                var (success, errorMessage) = await db.IsConnectionWorksAsync();
                if (!success)
                {
                    await console.WriteErrorAsync(errorMessage);
                    return;
                }

                var databaseNames = await db.GetDatabaseNamesAsync();
                databaseName = console.GetSelect("Database name", databaseNames);
            }

            var databaseConnectionString = DbUtils.GetConnectionString(databaseType, databaseHost, isDatabaseDefaultPort, databasePort, databaseUserName, databasePassword, databaseName);

            var isProtectData = console.GetYesNo("Protect settings in sscms.json?");
            _settingsManager.SaveSettings(isProtectData, false, false, databaseType, databaseConnectionString, string.Empty, string.Empty, null, null, false, null);

            await console.WriteSuccessAsync("SSCMS was download and ready for install, please run: sscms install database");

        }
    }
}
