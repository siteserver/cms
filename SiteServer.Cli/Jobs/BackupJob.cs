using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Jobs
{
    public class BackupJob
    {
        public const string CommandName = "backup";

        private string _directory;
        private string _configFile;
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
        }

        public void PrintUsage()
        {
            Console.WriteLine("数据库备份: siteserver backup");
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

            if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
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

            await Backup(_includes, _excludes, _maxRows, treeInfo);

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功备份数据库至文件夹：{treeInfo.DirectoryPath}！");
        }

        public static async Task Backup(List<string> includes, List<string> excludes, int maxRows, TreeInfo treeInfo)
        {
            var allTableNames = DataProvider.DatabaseDao.GetTableNameList();
            var tableNames = new List<string>();

            foreach (var tableName in allTableNames)
            {
                if (includes != null && !StringUtils.ContainsIgnoreCase(includes, tableName)) continue;
                if (StringUtils.ContainsIgnoreCase(excludes, tableName)) continue;
                if (StringUtils.ContainsIgnoreCase(tableNames, tableName)) continue;
                tableNames.Add(tableName);
            }

            await FileUtils.WriteTextAsync(treeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(tableNames));

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("备份表名称", "总条数");
            await CliUtils.PrintRowLineAsync();

            foreach (var tableName in tableNames)
            {
                var tableInfo = new TableInfo
                {
                    Columns = DataProvider.DatabaseDao.GetTableColumnInfoList(WebConfigUtils.ConnectionString, tableName),
                    TotalCount = DataProvider.DatabaseDao.GetCount(tableName),
                    RowFiles = new List<string>()
                };

                if (maxRows > 0 && tableInfo.TotalCount > maxRows)
                {
                    tableInfo.TotalCount = maxRows;
                }

                await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                var identityColumnName = DataProvider.DatabaseDao.AddIdentityColumnIdIfNotExists(tableName, tableInfo.Columns);

                if (tableInfo.TotalCount > 0)
                {
                    var current = 1;
                    if (tableInfo.TotalCount > CliUtils.PageSize)
                    {
                        var pageCount = (int) Math.Ceiling((double) tableInfo.TotalCount / CliUtils.PageSize);

                        using (var progress = new ProgressBar())
                        {
                            for (; current <= pageCount; current++)
                            {
                                progress.Report((double) (current - 1) / pageCount);

                                var fileName = $"{current}.json";
                                tableInfo.RowFiles.Add(fileName);
                                var offset = (current - 1) * CliUtils.PageSize;
                                var limit = tableInfo.TotalCount - offset < CliUtils.PageSize
                                    ? tableInfo.TotalCount - offset
                                    : CliUtils.PageSize;

                                var rows = DataProvider.DatabaseDao.GetPageObjects(tableName, identityColumnName, offset,
                                    limit);

                                await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName),
                                    Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = DataProvider.DatabaseDao.GetObjects(tableName);

                        await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8,
                            TranslateUtils.JsonSerialize(rows));
                    }
                }

                await FileUtils.WriteTextAsync(treeInfo.GetTableMetadataFilePath(tableName), Encoding.UTF8,
                    TranslateUtils.JsonSerialize(tableInfo));
            }
        }
    }
}
