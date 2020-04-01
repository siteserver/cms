using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Jobs
{
    public class SyncJob
    {
        public const string CommandName = "sync";

        private string _directory;
        private string _from;
        private string _to;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private bool _isHelp;

        private readonly OptionSet _options;

        public SyncJob()
        {
            _options = new OptionSet() {
                { "d|directory=", "指定保存备份文件的文件夹名称",
                    v => _directory = v },
                { "from=", "指定需要备份的配置文件Web.config路径或文件名",
                    v => _from = v },
                { "to=", "指定需要恢复的配置文件Web.config路径或文件名",
                    v => _to = v },
                { "includes=", "指定需要备份的表，多个表用英文逗号隔开，默认备份所有表",
                    v => _includes = v == null ? null : TranslateUtils.StringCollectionToStringList(v) },
                { "excludes=", "指定需要排除的表，多个表用英文逗号隔开",
                    v => _excludes = v == null ? null : TranslateUtils.StringCollectionToStringList(v) },
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

        public async Task Execute(IJobContext context)
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

            var treeInfo = new TreeInfo(directory);
            DirectoryUtils.CreateDirectoryIfNotExists(treeInfo.DirectoryPath);

            var backupWebConfigPath = CliUtils.GetWebConfigPath(_from);
            if (!FileUtils.IsFileExists(backupWebConfigPath))
            {
                await CliUtils.PrintErrorAsync($"系统配置文件不存在：{backupWebConfigPath}！");
                return;
            }
            WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, backupWebConfigPath);
            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync($"{backupWebConfigPath} 中数据库连接字符串 connectionString 未设置");
                return;
            }

            await Console.Out.WriteLineAsync($"备份数据库类型: {WebConfigUtils.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"备份连接字符串: {WebConfigUtils.ConnectionString}");
            await Console.Out.WriteLineAsync($"备份文件夹: {treeInfo.DirectoryPath}");

            if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync($"系统无法连接到 {backupWebConfigPath} 中设置的数据库");
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

            var errorLogFilePath = CliUtils.CreateErrorLogFile(CommandName);

            await BackupJob.Backup(_includes, _excludes, _maxRows, treeInfo);

            var restoreWebConfigPath = CliUtils.GetWebConfigPath(_to);
            if (!FileUtils.IsFileExists(restoreWebConfigPath))
            {
                await CliUtils.PrintErrorAsync($"系统配置文件不存在：{restoreWebConfigPath}！");
                return;
            }
            WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, restoreWebConfigPath);
            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync($"{restoreWebConfigPath} 中数据库连接字符串 connectionString 未设置");
                return;
            }
            await Console.Out.WriteLineAsync($"恢复数据库类型: {WebConfigUtils.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"恢复连接字符串: {WebConfigUtils.ConnectionString}");
            if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync($"系统无法连接到 {restoreWebConfigPath} 中设置的数据库");
                return;
            }

            await RestoreJob.Restore(_includes, _excludes, true, treeInfo.DirectoryPath, treeInfo, errorLogFilePath);

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync("恭喜，成功同步数据！");
        }
    }
}
