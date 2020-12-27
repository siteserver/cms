using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataRestoreJob : IJobService
    {
        public string CommandName => "data restore";

        private string _directory;
        private List<string> _includes;
        private List<string> _excludes;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDataRestoreService _restoreService;
        private readonly OptionSet _options;

        public DataRestoreJob(ISettingsManager settingsManager, IDataRestoreService restoreService)
        {
            _settingsManager = settingsManager;
            _restoreService = restoreService;

            _options = new OptionSet {
                { "d|directory=", "Restore folder name",
                    v => _directory = v },
                { "includes=", "Include table names, separated by commas, default restore all tables",
                    v => _includes = v == null ? null : ListUtils.GetStringList(v) },
                { "excludes=", "Exclude table names, separated by commas",
                    v => _excludes = v == null ? null : ListUtils.GetStringList(v) },
                { "h|help",  "Display help",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: restore backup files to database");
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

            if (string.IsNullOrEmpty(_directory))
            {
                await WriteUtils.PrintErrorAsync("Restore folder name not specified: --directory");
                return;
            }

            var treeInfo = new TreeInfo(_settingsManager, _directory);

            if (!DirectoryUtils.IsDirectoryExists(treeInfo.DirectoryPath))
            {
                await WriteUtils.PrintErrorAsync($"恢复数据的文件夹 {treeInfo.DirectoryPath} 不存在");
                return;
            }

            var tablesFilePath = treeInfo.TablesFilePath;
            if (!FileUtils.IsFileExists(tablesFilePath))
            {
                await WriteUtils.PrintErrorAsync($"恢复文件 {treeInfo.TablesFilePath} 不存在");
                return;
            }

            var configPath = CliUtils.GetConfigPath(_settingsManager);
            if (!FileUtils.IsFileExists(configPath))
            {
                await WriteUtils.PrintErrorAsync($"The sscms.json file does not exist: {configPath}");
                return;
            }

            //WebConfigUtils.Load(_settingsManager.ContentRootPath, webConfigPath);

            //if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            //{
            //    await CliUtils.PrintErrorAsync($"{webConfigPath} 中数据库连接字符串 connectionString 未设置");
            //    return;
            //}

            //await Console.Out.WriteLineAsync($"数据库类型: {_settingsManager.Database.DatabaseType.GetValue()}");
            //await Console.Out.WriteLineAsync($"连接字符串: {WebConfigUtils.ConnectionString}");
            //await Console.Out.WriteLineAsync($"恢复文件夹: {treeInfo.DirectoryPath}");

            await Console.Out.WriteLineAsync($"Database type: {_settingsManager.Database.DatabaseType.GetDisplayName()}");
            await Console.Out.WriteLineAsync($"Database connection string: {_settingsManager.Database.ConnectionString}");
            await Console.Out.WriteLineAsync($"Restore folder: {treeInfo.DirectoryPath}");

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await WriteUtils.PrintErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            //if (!_dataOnly)
            //{
            //    if (!await _configRepository.IsNeedInstallAsync())
            //    {
            //        await WriteUtils.PrintErrorAsync("The data could not be restored on the installed sscms database");
            //        return;
            //    }

            //    // 恢复前先创建表，确保系统在恢复的数据库中能够使用
            //    //await _databaseManager.CreateSiteServerTablesAsync();

            //    if (_settingsManager.DatabaseType == DatabaseType.SQLite)
            //    {
            //        var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.DefaultLocalDbFileName);
            //        if (!FileUtils.IsFileExists(filePath))
            //        {
            //            await FileUtils.WriteTextAsync(filePath, string.Empty);
            //        }
            //    }

            //    await _databaseManager.SyncDatabaseAsync();
            //}

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintRowAsync("Restore table name", "Count");
            await WriteUtils.PrintRowLineAsync();

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);

            var errorTableNames = await _restoreService.RestoreAsync(_includes, _excludes, tablesFilePath, treeInfo, errorLogFilePath);

            if (errorTableNames.Count == 0)
            {
                await WriteUtils.PrintSuccessAsync("restore database successfully!");
            }
            else
            {
                await WriteUtils.PrintErrorAsync($"Database restore failed and the following table was not successfully restored: {ListUtils.ToString(errorTableNames)}");
            }
        }
    }
}
