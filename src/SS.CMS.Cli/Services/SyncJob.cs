using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using Mono.Options;
using SS.CMS.Abstractions;
using SS.CMS.Cli.Core;

namespace SS.CMS.Cli.Services
{
    public class SyncJob : IJobService
    {
        public string CommandName => "sync";

        private string _directory;
        private string _from;
        private string _to;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly RestoreJob _restoreJob;
        private readonly OptionSet _options;

        public SyncJob(ISettingsManager settingsManager, IDatabaseManager databaseManager, RestoreJob restoreJob)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _restoreJob = restoreJob;

            _options = new OptionSet
            {
                { "d|directory=", "指定保存备份文件的文件夹名称",
                    v => _directory = v },
                { "from=", "指定需要备份的配置文件Web.config路径或文件名",
                    v => _from = v },
                { "to=", "指定需要恢复的配置文件Web.config路径或文件名",
                    v => _to = v },
                { "includes=", "指定需要备份的表，多个表用英文逗号隔开，默认备份所有表",
                    v => _includes = v == null ? null : Utilities.GetStringList(v) },
                { "excludes=", "指定需要排除的表，多个表用英文逗号隔开",
                    v => _excludes = v == null ? null : Utilities.GetStringList(v) },
                { "max-rows=", "指定需要备份的表的最大行数",
                    v => _maxRows = v == null ? 0 : TranslateUtils.ToInt(v) },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine("数据库同步: siteserver sync");
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

            var directory = _directory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = $"backup/{DateTime.Now:yyyy-MM-dd}";
            }

            var treeInfo = new TreeInfo(_settingsManager, directory);
            DirectoryUtils.CreateDirectoryIfNotExists(treeInfo.DirectoryPath);

            var backupWebConfigPath = CliUtils.GetWebConfigPath(_from, _settingsManager);
            if (!FileUtils.IsFileExists(backupWebConfigPath))
            {
                await CliUtils.PrintErrorAsync($"系统配置文件不存在：{backupWebConfigPath}！");
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
                await CliUtils.PrintErrorAsync($"数据库连接错误：{errorMessage}");
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

            var errorLogFilePath = CliUtils.CreateErrorLogFile(CommandName, _settingsManager);

            await BackupJob.Backup(_settingsManager, _databaseManager, _includes, _excludes, _maxRows, treeInfo);

            var restoreWebConfigPath = CliUtils.GetWebConfigPath(_to, _settingsManager);
            if (!FileUtils.IsFileExists(restoreWebConfigPath))
            {
                await CliUtils.PrintErrorAsync($"系统配置文件不存在：{restoreWebConfigPath}！");
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
                await CliUtils.PrintErrorAsync($"数据库连接错误：{errorMessage}");
                return;
            }

            await _restoreJob.RestoreAsync(_includes, _excludes, true, treeInfo.DirectoryPath, treeInfo, errorLogFilePath);

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync("恭喜，成功同步数据！");
        }
    }
}
