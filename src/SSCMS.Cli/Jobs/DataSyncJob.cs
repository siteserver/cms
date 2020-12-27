using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataSyncJob : IJobService
    {
        public string CommandName => "data sync";

        private string _directory;
        private string _from;
        private string _to;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private int _pageSize = CliConstants.DefaultPageSize;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IDataRestoreService _restoreService;
        private readonly OptionSet _options;

        public DataSyncJob(ISettingsManager settingsManager, IDatabaseManager databaseManager,
            IDataRestoreService restoreService)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _restoreService = restoreService;

            _options = new OptionSet
            {
                {
                    "d|directory=", "Backup folder name",
                    v => _directory = v
                },
                {
                    "from=", "Specify the path or file name of sscms.json configuration file that you want to backup",
                    v => _from = v
                },
                {
                    "to=", "Specify the path or file name of sscms.json configuration file that you want to restore",
                    v => _to = v
                },
                {
                    "includes=", "Include table names, separated by commas, default backup all tables",
                    v => _includes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "excludes=", "Exclude table names, separated by commas",
                    v => _excludes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "max-rows=", "Maximum number of rows to backup, all data is backed up by default",
                    v => _maxRows = v == null ? 0 : TranslateUtils.ToInt(v)
                },
                {
                    "page-size=", "The number of rows fetch at a time, 1000 by default",
                    v => _pageSize = v == null ? CliConstants.DefaultPageSize : TranslateUtils.ToInt(v)
                },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: sync backup files to database");
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

            var directory = _directory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = $"backup/{DateTime.Now:yyyy-MM-dd}";
            }

            var treeInfo = new TreeInfo(_settingsManager, directory);
            DirectoryUtils.CreateDirectoryIfNotExists(treeInfo.DirectoryPath);

            var backupConfigPath = PathUtils.Combine(_settingsManager.ContentRootPath, _from);
            if (!FileUtils.IsFileExists(backupConfigPath))
            {
                await WriteUtils.PrintErrorAsync($"The sscms configuration file does not exist: {backupConfigPath}");
                return;
            }
            //WebConfigUtils.Load(_settingsManager.ContentRootPath, backupWebConfigPath);
            //if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            //{
            //    await CliUtils.PrintErrorAsync($"{backupWebConfigPath} 中数据库连接字符串 connectionString 未设置");
            //    return;
            //}

            //await Console.Out.WriteLineAsync($"备份数据库类型: {_settingsManager.Database.DatabaseType.GetValue()}");
            //await Console.Out.WriteLineAsync($"备份连接字符串: {WebConfigUtils.ConnectionString}");
            //await Console.Out.WriteLineAsync($"备份文件夹: {treeInfo.DirectoryPath}");

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await WriteUtils.PrintErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            if (_excludes == null)
            {
                _excludes = new List<string>();
            }
            _excludes.Add("bairong_Log");
            _excludes.Add("bairong_ErrorLog");
            _excludes.Add("siteserver_ErrorLog");
            _excludes.Add("siteserver_Log");
            _excludes.Add("siteserver_Tracking");

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
            await DataBackupJob.Backup(_settingsManager, _databaseManager, _includes, _excludes, _maxRows, _pageSize, treeInfo, errorLogFilePath);

            var restoreConfigPath = PathUtils.Combine(_settingsManager.ContentRootPath, _to);
            if (!FileUtils.IsFileExists(restoreConfigPath))
            {
                await WriteUtils.PrintErrorAsync($"The sscms configuration file does not exist: {restoreConfigPath}");
                return;
            }
            //WebConfigUtils.Load(_settingsManager.ContentRootPath, restoreWebConfigPath);
            //if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            //{
            //    await CliUtils.PrintErrorAsync($"{restoreWebConfigPath} 中数据库连接字符串 connectionString 未设置");
            //    return;
            //}
            //await Console.Out.WriteLineAsync($"恢复数据库类型: {_settingsManager.Database.DatabaseType.GetValue()}");
            //await Console.Out.WriteLineAsync($"恢复连接字符串: {WebConfigUtils.ConnectionString}");
            (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await WriteUtils.PrintErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            await _restoreService.RestoreAsync(_includes, _excludes, treeInfo.DirectoryPath, treeInfo, errorLogFilePath);

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintSuccessAsync("sync database successfully!");
        }
    }
}
