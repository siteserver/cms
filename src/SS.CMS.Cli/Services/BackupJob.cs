using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using SS.CMS.Cli.Core;
using SS.CMS.Data;
using SS.CMS.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Cli.Services
{
    public class BackupJob
    {
        public const string CommandName = "backup";

        public static async Task Execute(IJobContext context)
        {
            var application = CliUtils.Provider.GetService<BackupJob>();
            await application.RunAsync(context);
        }

        public static void PrintUsage()
        {
            Console.WriteLine("数据库备份: siteserver backup");
            var job = new BackupJob();
            job._options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        private string _directory;
        private string _configFile;
        private string _databaseType;
        private string _connectionString;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private bool _isHelp;
        private readonly OptionSet _options;

        public BackupJob()
        {
            _options = new OptionSet() {
                { "d|directory=", "指定保存备份文件的文件夹名称",
                    v => _directory = v },
                { "c|config-file=", "指定配置文件appSettings.json路径或文件名",
                    v => _configFile = v },
                { "database-type=", "指定需要备份的数据库类型",
                    v => _databaseType = v },
                { "connection-string=", "指定需要备份的数据库连接字符串",
                    v => _connectionString = v },
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

        public async Task RunAsync(IJobContext context)
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

            var (db, errorMessage) = CliUtils.GetDatabase(_databaseType, _connectionString, _configFile);
            if (db == null)
            {
                await CliUtils.PrintErrorAsync(errorMessage);
                return;
            }

            await Console.Out.WriteLineAsync($"数据库类型: {db.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"连接字符串: {db.ConnectionString}");
            await Console.Out.WriteLineAsync($"备份文件夹: {treeInfo.DirectoryPath}");

            if (_excludes == null)
            {
                _excludes = new List<string>();
            }
            _excludes.Add("bairong_Log");
            _excludes.Add("bairong_ErrorLog");
            _excludes.Add("siteserver_ErrorLog");
            _excludes.Add("siteserver_Log");
            _excludes.Add("siteserver_Tracking");

            var allTableNames = await db.GetTableNamesAsync();
            var tableNames = new List<string>();

            foreach (var tableName in allTableNames)
            {
                if (_includes != null && !StringUtils.ContainsIgnoreCase(_includes, tableName)) continue;
                if (StringUtils.ContainsIgnoreCase(_excludes, tableName)) continue;
                if (StringUtils.ContainsIgnoreCase(tableNames, tableName)) continue;
                tableNames.Add(tableName);
            }

            await FileUtils.WriteTextAsync(treeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(tableNames));

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("备份表名称", "总条数");
            await CliUtils.PrintRowLineAsync();

            foreach (var tableName in tableNames)
            {
                var repository = new Repository(db, tableName);
                var tableInfo = new TableInfo
                {
                    Columns = await db.GetTableColumnsAsync(tableName),
                    TotalCount = await repository.CountAsync(),
                    RowFiles = new List<string>()
                };

                if (_maxRows > 0 && tableInfo.TotalCount > _maxRows)
                {
                    tableInfo.TotalCount = _maxRows;
                }

                await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                var identityColumnName = await db.AddIdentityColumnIdIfNotExistsAsync(tableName, tableInfo.Columns);

                if (tableInfo.TotalCount > 0)
                {
                    var current = 1;
                    if (tableInfo.TotalCount > CliUtils.PageSize)
                    {
                        var pageCount = (int)Math.Ceiling((double)tableInfo.TotalCount / CliUtils.PageSize);

                        using (var progress = new ProgressBar())
                        {
                            for (; current <= pageCount; current++)
                            {
                                progress.Report((double)(current - 1) / pageCount);

                                var fileName = $"{current}.json";
                                tableInfo.RowFiles.Add(fileName);
                                var offset = (current - 1) * CliUtils.PageSize;
                                var limit = tableInfo.TotalCount - offset < CliUtils.PageSize ? tableInfo.TotalCount - offset : CliUtils.PageSize;

                                var rows = await repository.GetAllAsync<IEnumerable<dynamic>>(Q.Offset(offset).Limit(limit).OrderBy(identityColumnName));

                                await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = await repository.GetAllAsync<IEnumerable<dynamic>>(Q.OrderBy(identityColumnName));

                        await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                    }
                }

                await FileUtils.WriteTextAsync(treeInfo.GetTableMetadataFilePath(tableName), Encoding.UTF8, TranslateUtils.JsonSerialize(tableInfo));
            }

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功备份数据库至文件夹：{treeInfo.DirectoryPath}！");
        }
    }
}
