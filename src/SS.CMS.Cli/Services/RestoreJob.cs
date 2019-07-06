using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Mono.Options;
using Newtonsoft.Json.Linq;
using SS.CMS.Cli.Core;
using SS.CMS.Data;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Cli.Services
{
    public class RestoreJob
    {
        public const string CommandName = "restore";

        public static async Task Execute(IJobContext context)
        {
            var application = CliUtils.Provider.GetService<RestoreJob>();
            await application.RunAsync(context);
        }

        public static void PrintUsage()
        {
            Console.WriteLine("数据库恢复: siteserver restore");
            var job = new RestoreJob(null, null);
            job._options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        private string _directory;
        private string _configFile;
        private string _databaseType;
        private string _connectionString;
        private List<string> _includes;
        private List<string> _excludes;
        private bool _dataOnly;
        private bool _isHelp;
        private readonly OptionSet _options;
        private IConfigRepository _configRepository;
        private ITableManager _tableManager;
        public RestoreJob(IConfigRepository configRepository, ITableManager tableManager)
        {
            _configRepository = configRepository;
            _tableManager = tableManager;

            _options = new OptionSet {
                { "d|directory=", "从指定的文件夹中恢复数据",
                    v => _directory = v },
                { "c|config-file=", "指定配置文件Web.config路径或文件名",
                    v => _configFile = v },
                { "database-type=", "指定需要还原的数据库类型",
                    v => _databaseType = v },
                { "connection-string=", "指定需要还原的数据库连接字符串",
                    v => _connectionString = v },
                { "includes=", "指定需要还原的表，多个表用英文逗号隔开，默认还原所有表",
                    v => _includes = v == null ? null : TranslateUtils.StringCollectionToStringList(v) },
                { "excludes=", "指定需要排除的表，多个表用英文逗号隔开",
                    v => _excludes = v == null ? null : TranslateUtils.StringCollectionToStringList(v) },
                { "data-only",  "仅恢复数据",
                    v => _dataOnly = v != null },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public async Task RunAsync(IJobContext context)
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

            var treeInfo = new TreeInfo(_directory);

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

            var (db, errorMessage) = CliUtils.GetDatabase(_databaseType, _connectionString, _configFile);
            if (db == null)
            {
                await CliUtils.PrintErrorAsync(errorMessage);
                return;
            }

            await Console.Out.WriteLineAsync($"数据库类型: {db.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"连接字符串: {db.ConnectionString}");
            await Console.Out.WriteLineAsync($"恢复文件夹: {treeInfo.DirectoryPath}");

            if (!_dataOnly)
            {
                var configInfo = await _configRepository.GetConfigInfoAsync();
                if (configInfo != null)
                {
                    await CliUtils.PrintErrorAsync("数据无法在已安装系统的数据库中恢复，命令执行失败");
                    return;
                }

                // 恢复前先创建表，确保系统在恢复的数据库中能够使用
                await _tableManager.SyncSystemTablesAsync();
            }

            var tableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(tablesFilePath, Encoding.UTF8));

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("恢复表名称", "总条数");
            await CliUtils.PrintRowLineAsync();

            var errorLogFilePath = CliUtils.CreateErrorLogFile(CommandName);

            foreach (var tableName in tableNames)
            {
                try
                {
                    var repository = new Repository(db, tableName);

                    if (_includes != null)
                    {
                        if (!StringUtils.ContainsIgnoreCase(_includes, tableName)) continue;
                    }
                    if (_excludes != null)
                    {
                        if (StringUtils.ContainsIgnoreCase(_excludes, tableName)) continue;
                    }

                    var metadataFilePath = treeInfo.GetTableMetadataFilePath(tableName);

                    if (!FileUtils.IsFileExists(metadataFilePath)) continue;

                    var tableInfo = TranslateUtils.JsonDeserialize<TableInfo>(await FileUtils.ReadTextAsync(metadataFilePath, Encoding.UTF8));

                    await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                    if (!await db.IsTableExistsAsync(tableName))
                    {
                        try
                        {
                            await db.CreateTableAsync(tableName, tableInfo.Columns);
                        }
                        catch (Exception ex)
                        {
                            await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                            {
                                DateTime = DateTime.Now,
                                Detail = $"创建表 {tableName}",
                                Exception = ex
                            });

                            continue;
                        }
                    }
                    else
                    {
                        await _tableManager.AlterSystemTableAsync(tableName, tableInfo.Columns);
                    }

                    if (tableInfo.RowFiles.Count > 0)
                    {
                        using (var progress = new ProgressBar())
                        {
                            for (var i = 0; i < tableInfo.RowFiles.Count; i++)
                            {
                                progress.Report((double)i / tableInfo.RowFiles.Count);

                                var fileName = tableInfo.RowFiles[i];

                                var objects = TranslateUtils.JsonDeserialize<List<JObject>>(
                                    await FileUtils.ReadTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName),
                                        Encoding.UTF8));

                                try
                                {
                                    await repository.BulkInsertAsync(objects);
                                }
                                catch (Exception ex)
                                {
                                    await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                                    {
                                        DateTime = DateTime.Now,
                                        Detail = $"插入表 {tableName}, 文件名 {fileName}",
                                        Exception = ex
                                    });
                                }
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

            if (db.DatabaseType == DatabaseType.Oracle)
            {
                var tableNameList = await db.GetTableNamesAsync();
                foreach (var tableName in tableNameList)
                {
                    try
                    {
                        var sqlString =
                            $"ALTER TABLE {tableName} MODIFY Id GENERATED ALWAYS AS IDENTITY(START WITH LIMIT VALUE)";

                        using (var connection = db.GetConnection())
                        {
                            await connection.ExecuteAsync(sqlString);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            if (!_dataOnly)
            {
                // 恢复后同步表，确保内容辅助表字段与系统一致
                _tableManager.SyncContentTables();
                await _tableManager.UpdateConfigVersionAsync();
            }

            await Console.Out.WriteLineAsync($"恭喜，成功从文件夹：{treeInfo.DirectoryPath} 恢复数据！");
        }
    }
}
