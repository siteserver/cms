using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InstallDownloadJob : IJobService
    {
        public string CommandName => "install download";

        private bool _isNightly;
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
                { "nightly",  "Install nightly version",
                    v => _isNightly = v != null },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: download sscms and save settings");
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

            var proceed = ReadUtils.GetYesNo($"Do you want to install SS CMS in {contentRootPath}?");
            if (!proceed) return;

            InstallUtils.Init(contentRootPath);

            if (!CliUtils.IsSsCmsExists(contentRootPath))
            {
                var (success, result, errorMessage) = _apiService.GetReleases(_isNightly, string.Empty, null);
                if (!success)
                {
                    await WriteUtils.PrintErrorAsync(errorMessage);
                    return;
                }

                Console.WriteLine($"Downloading {result.Cms.Version}...");
                CloudUtils.Dl.DownloadCms(_pathManager, result.Cms.Version);

                var name = CloudUtils.Dl.GetCmsDownloadName(result.Cms.Version);
                var packagePath = _pathManager.GetPackagesPath(name);

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
            }

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
            _settingsManager.SaveSettings(_isNightly, isProtectData, databaseType, databaseConnectionString, string.Empty);

            await WriteUtils.PrintSuccessAsync("SS CMS was download and ready for install, please run sscms-cli install sscms");
        }
    }
}
