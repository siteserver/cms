using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataRestoreJob : IJobService
    {
        public string CommandName => "data restore";

        private string _directory;
        private string _configFile;
        private List<string> _includes;
        private List<string> _excludes;
        private bool _dataOnly;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDatabaseManager _databaseManager;
        private readonly IDataRestoreService _restoreService;
        private readonly OptionSet _options;

        public DataRestoreJob(ISettingsManager settingsManager, IConfigRepository configRepository, IDatabaseManager databaseManager, IDataRestoreService restoreService)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _databaseManager = databaseManager;
            _restoreService = restoreService;

            _options = new OptionSet {
                { "d|directory=", "从指定的文件夹中恢复数据",
                    v => _directory = v },
                { "c|config-file=", "指定配置文件Web.config路径或文件名",
                    v => _configFile = v },
                { "includes=", "指定需要还原的表，多个表用英文逗号隔开，默认还原所有表",
                    v => _includes = v == null ? null : Utilities.GetStringList(v) },
                { "excludes=", "指定需要排除的表，多个表用英文逗号隔开",
                    v => _excludes = v == null ? null : Utilities.GetStringList(v) },
                { "data-only",  "仅恢复数据",
                    v => _dataOnly = v != null },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: restore backup files to database");
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

            if (string.IsNullOrEmpty(_directory))
            {
                await WriteUtils.PrintErrorAsync("需要指定恢复数据的文件夹名称：directory");
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

            var webConfigPath = CliUtils.GetWebConfigPath(_configFile, _settingsManager);
            if (!FileUtils.IsFileExists(webConfigPath))
            {
                await WriteUtils.PrintErrorAsync($"系统配置文件不存在：{webConfigPath}！");
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

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await WriteUtils.PrintErrorAsync($"数据库连接错误：{errorMessage}");
                return;
            }

            if (!_dataOnly)
            {
                if (!await _configRepository.IsNeedInstallAsync())
                {
                    await WriteUtils.PrintErrorAsync("数据无法在已安装系统的数据库中恢复，命令执行失败");
                    return;
                }

                // 恢复前先创建表，确保系统在恢复的数据库中能够使用
                await _databaseManager.CreateSiteServerTablesAsync();
            }

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintRowAsync("恢复表名称", "总条数");
            await WriteUtils.PrintRowLineAsync();

            var errorLogFilePath = CliUtils.CreateErrorLogFile(CommandName, _settingsManager);

            await _restoreService.RestoreAsync(_includes, _excludes, _dataOnly, tablesFilePath, treeInfo, errorLogFilePath);

            await Console.Out.WriteLineAsync($"恭喜，成功从文件夹：{treeInfo.DirectoryPath} 恢复数据！");
        }
    }
}
