using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Datory;
using Datory.Utils;
using Mono.Options;
using Newtonsoft.Json.Linq;
using SS.CMS.Abstractions;
using SS.CMS.Cli.Core;

namespace SS.CMS.Cli.Services
{
    public class RestoreJob : IJobService
    {
        public string CommandName => "restore";

        private string _directory;
        private string _configFile;
        private List<string> _includes;
        private List<string> _excludes;
        private bool _dataOnly;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IDatabaseManager _databaseManager;
        private readonly OptionSet _options;

        public RestoreJob(ISettingsManager settingsManager, IConfigRepository configRepository, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _databaseManager = databaseManager;

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
            Console.WriteLine("数据库恢复: siteserver restore");
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
                await CliUtils.PrintErrorAsync("需要指定恢复数据的文件夹名称：directory");
                return;
            }

            var treeInfo = new TreeInfo(_settingsManager, _directory);

            if (!DirectoryUtils.IsDirectoryExists(treeInfo.DirectoryPath))
            {
                await CliUtils.PrintErrorAsync($"恢复数据的文件夹 {treeInfo.DirectoryPath} 不存在");
                return;
            }

            var tablesFilePath = treeInfo.TablesFilePath;
            if (!FileUtils.IsFileExists(tablesFilePath))
            {
                await CliUtils.PrintErrorAsync($"恢复文件 {treeInfo.TablesFilePath} 不存在");
                return;
            }

            var webConfigPath = CliUtils.GetWebConfigPath(_configFile, _settingsManager);
            if (!FileUtils.IsFileExists(webConfigPath))
            {
                await CliUtils.PrintErrorAsync($"系统配置文件不存在：{webConfigPath}！");
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
                await CliUtils.PrintErrorAsync($"数据库连接错误：{errorMessage}");
                return;
            }

            if (!_dataOnly)
            {
                if (!await _configRepository.IsNeedInstallAsync())
                {
                    await CliUtils.PrintErrorAsync("数据无法在已安装系统的数据库中恢复，命令执行失败");
                    return;
                }

                // 恢复前先创建表，确保系统在恢复的数据库中能够使用
                await _databaseManager.CreateSiteServerTablesAsync();
            }

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("恢复表名称", "总条数");
            await CliUtils.PrintRowLineAsync();

            var errorLogFilePath = CliUtils.CreateErrorLogFile(CommandName, _settingsManager);

            await RestoreAsync(_includes, _excludes, _dataOnly, tablesFilePath, treeInfo, errorLogFilePath);

            await Console.Out.WriteLineAsync($"恭喜，成功从文件夹：{treeInfo.DirectoryPath} 恢复数据！");
        }

        public async Task RestoreAsync(List<string> includes, List<string> excludes, bool dataOnly, string tablesFilePath, TreeInfo treeInfo, string errorLogFilePath)
        {
            var tableNames =
                TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(tablesFilePath, Encoding.UTF8));

            foreach (var tableName in tableNames)
            {
                try
                {
                    if (includes != null)
                    {
                        if (!StringUtils.ContainsIgnoreCase(includes, tableName)) continue;
                    }

                    if (excludes != null)
                    {
                        if (StringUtils.ContainsIgnoreCase(excludes, tableName)) continue;
                    }

                    var metadataFilePath = treeInfo.GetTableMetadataFilePath(tableName);

                    if (!FileUtils.IsFileExists(metadataFilePath)) continue;

                    var tableInfo =
                        TranslateUtils.JsonDeserialize<TableInfo>(
                            await FileUtils.ReadTextAsync(metadataFilePath, Encoding.UTF8));

                    await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                    if (await _settingsManager.Database.IsTableExistsAsync(tableName))
                    {
                        await _settingsManager.Database.DropTableAsync(tableName);
                    }

                    await _settingsManager.Database.CreateTableAsync(tableName, tableInfo.Columns);

                    if (tableInfo.RowFiles.Count > 0)
                    {
                        using var progress = new ProgressBar();
                        for (var i = 0; i < tableInfo.RowFiles.Count; i++)
                        {
                            progress.Report((double)i / tableInfo.RowFiles.Count);

                            var fileName = tableInfo.RowFiles[i];

                            var objects = TranslateUtils.JsonDeserialize<List<JObject>>(
                                await FileUtils.ReadTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8));

                            try
                            {
                                var repository = new Repository(_settingsManager.Database, tableName,
                                    tableInfo.Columns);
                                await repository.BulkInsertAsync(objects);
                            }
                            catch (Exception exception)
                            {
                                await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                                {
                                    DateTime = DateTime.Now,
                                    Detail = $"插入表 {tableName}, 文件名 {fileName}",
                                    Exception = exception
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                    {
                        DateTime = DateTime.Now,
                        Detail = $"插入表 {tableName}",
                        Exception = ex
                    });
                }
            }

            await CliUtils.PrintRowLineAsync();

            if (_settingsManager.Database.DatabaseType == DatabaseType.Oracle)
            {
                var database = _databaseManager.GetDatabase();
                var allTableNames = await database.GetTableNamesAsync();
                foreach (var tableName in allTableNames)
                {
                    try
                    {
                        var sqlString =
                            $"ALTER TABLE {tableName} MODIFY Id GENERATED ALWAYS AS IDENTITY(START WITH LIMIT VALUE)";

                        using (var connection = _settingsManager.Database.GetConnection())
                        {
                            await connection.ExecuteAsync(sqlString);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                //foreach (var tableName in tableNameList)
                //{
                //    DataProvider.DatabaseRepository.AlterOracleAutoIncresementIdToMaxValue(tableName);
                //}
            }

            if (!dataOnly)
            {
                // 恢复后同步表，确保内容辅助表字段与系统一致
                await _databaseManager.SyncContentTablesAsync();
                await _configRepository.UpdateConfigVersionAsync(_settingsManager.ProductVersion);
            }
        }
    }
}
