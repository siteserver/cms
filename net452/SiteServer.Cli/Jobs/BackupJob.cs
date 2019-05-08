using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Fx;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Jobs
{
    public static class BackupJob
    {
        public const string CommandName = "backup";

        private static string _directory;
        private static string _configFile;
        private static List<string> _includes;
        private static List<string> _excludes;
        private static int _maxRows;
        private static bool _isHelp;

        private static readonly OptionSet Options = new OptionSet() {
            { "d|directory=", "指定保存备份文件的文件夹名称",
                v => _directory = v },
            { "c|config-file=", "指定配置文件Web.config路径或文件名",
                v => _configFile = v },
            { "includes=", "指定需要备份的表，多个表用英文逗号隔开，默认备份所有表",
                v => _includes = v == null ? null : TranslateUtils.StringCollectionToStringList(v) },
            { "excludes=", "指定需要排除的表，多个表用英文逗号隔开",
                v => _excludes = v == null ? null : TranslateUtils.StringCollectionToStringList(v) },
            { "max-rows=", "指定需要备份的表的最大行数",
                v => _maxRows = v == null ? 0 : TranslateUtils.ToInt(v) },
            { "h|help",  "命令说明",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("数据库备份: siteserver backup");
            Options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public static async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(Options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                _directory = $"backup/{DateTime.Now:yyyy-MM-dd}";
            }

            var treeInfo = new TreeInfo(_directory);
            DirectoryUtils.CreateDirectoryIfNotExists(treeInfo.DirectoryPath);

            var webConfigPath = CliUtils.GetWebConfigPath(_configFile);
            if (!FileUtils.IsFileExists(webConfigPath))
            {
                await CliUtils.PrintErrorAsync($"系统配置文件不存在：{webConfigPath}！");
                return;
            }

            WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, webConfigPath);

            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync($"{webConfigPath} 中数据库连接字符串 connectionString 未设置");
                return;
            }

            await Console.Out.WriteLineAsync($"数据库类型: {WebConfigUtils.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"连接字符串: {WebConfigUtils.ConnectionString}");
            await Console.Out.WriteLineAsync($"备份文件夹: {treeInfo.DirectoryPath}");

            if (!DataProvider.DatabaseApi.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync($"系统无法连接到 {webConfigPath} 中设置的数据库");
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

            var allTableNames = DatoryUtils.GetTableNames(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
            var tableNames = new List<string>();

            foreach (var tableName in allTableNames)
            {
                if (_includes != null && !StringUtils.ContainsIgnoreCase(_includes, tableName)) continue;
                if (StringUtils.ContainsIgnoreCase(_excludes, tableName)) continue;
                if (StringUtils.ContainsIgnoreCase(tableNames, tableName)) continue;
                tableNames.Add(tableName);
            }

            await FileUtils.WriteTextAsync(treeInfo.TablesFilePath, TranslateUtils.JsonSerialize(tableNames));

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("备份表名称", "总条数");
            await CliUtils.PrintRowLineAsync();

            foreach (var tableName in tableNames)
            {
                var tableInfo = new TableInfo
                {
                    Columns = DatoryUtils.GetTableColumns(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName),
                    TotalCount = DataProvider.DatabaseApi.GetCount(tableName),
                    RowFiles = new List<string>()
                };

                if (_maxRows > 0 && tableInfo.TotalCount > _maxRows)
                {
                    tableInfo.TotalCount = _maxRows;
                }

                await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                var identityColumnName = DatoryUtils.AddIdentityColumnIdIfNotExists(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName, tableInfo.Columns);

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

                                var rows = DataProvider.DatabaseApi.GetPageObjects(tableName, identityColumnName, offset, limit);

                                await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), TranslateUtils.JsonSerialize(rows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = DataProvider.DatabaseApi.GetObjects(tableName);

                        await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), TranslateUtils.JsonSerialize(rows));
                    }
                }

                await FileUtils.WriteTextAsync(treeInfo.GetTableMetadataFilePath(tableName), TranslateUtils.JsonSerialize(tableInfo));
            }

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功备份数据库至文件夹：{treeInfo.DirectoryPath}！");
        }
    }
}
