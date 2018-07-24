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
    public static class BackupJob
    {
        public const string CommandName = "backup";

        private static bool _isHelp;
        private static string _directory;
        private static string _configFileName;
        private static string _databaseType;
        private static string _connectionString;

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {cli.json} file name or {web.config} file name.",
                v => _configFileName = v },
            { "d|directory=", "the backup {directory} name.",
                v => _directory = v },
            { "database=", "the database type.",
                v => _databaseType = v },
            { "connection=", "the connection string.",
                v => _connectionString = v },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Backup command usage: siteserver backup");
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

            ConfigInfo configInfo;
            if (!string.IsNullOrEmpty(_databaseType) && !string.IsNullOrEmpty(_connectionString))
            {
                configInfo = CliUtils.LoadConfigByArgs(_databaseType, _connectionString);
            }
            else
            {
                configInfo = await CliUtils.LoadConfigByFileAsync(_configFileName);
            }
            
            if (configInfo == null)
            {
                await CliUtils.PrintErrorAsync("Error, config not exists");
                return;
            }

            if (configInfo.BackupConfig.Excludes == null)
            {
                configInfo.BackupConfig.Excludes = new List<string>();
            }
            configInfo.BackupConfig.Excludes.Add("bairong_ErrorLog");
            configInfo.BackupConfig.Excludes.Add("siteserver_ErrorLog");

            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync("Error, connection string is empty");
                return;
            }

            if (!DataProvider.DatabaseDao.IsConnectionStringWork(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                await CliUtils.PrintErrorAsync("Error, can not connect to the database");
                return;
            }

            await Console.Out.WriteLineAsync($"Database Type: {WebConfigUtils.DatabaseType.Value}");
            await Console.Out.WriteLineAsync($"Connection String: {WebConfigUtils.ConnectionString}");
            await Console.Out.WriteLineAsync($"Backup Directory: {treeInfo.DirectoryPath}");

            var tableNames = DataProvider.DatabaseDao.GetTableNameList();

            await FileUtils.WriteTextAsync(treeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(tableNames));

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("Backup Table Name", "Total Count");
            await CliUtils.PrintRowLineAsync();

            foreach (var tableName in tableNames)
            {
                if (configInfo.BackupConfig.Includes != null)
                {
                    if (!StringUtils.ContainsIgnoreCase(configInfo.BackupConfig.Includes, tableName)) continue;
                }
                if (configInfo.BackupConfig.Excludes != null)
                {
                    if (StringUtils.ContainsIgnoreCase(configInfo.BackupConfig.Excludes, tableName)) continue;
                }

                var tableInfo = new TableInfo
                {
                    Columns = DataProvider.DatabaseDao.GetTableColumnInfoList(WebConfigUtils.ConnectionString, tableName),
                    TotalCount = DataProvider.DatabaseDao.GetCount(tableName),
                    RowFiles = new List<string>()
                };

                await CliUtils.PrintRowAsync(tableName, tableInfo.TotalCount.ToString("#,0"));

                var identityColumnName = DataProvider.DatabaseDao.AddIdentityColumnIdIfNotExists(tableName, tableInfo.Columns);

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
                                var limit = CliUtils.PageSize;

                                var rows = DataProvider.DatabaseDao.GetPageObjects(tableName, identityColumnName, offset, limit);

                                await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = DataProvider.DatabaseDao.GetObjects(tableName);

                        await FileUtils.WriteTextAsync(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                    }
                }

                await FileUtils.WriteTextAsync(treeInfo.GetTableMetadataFilePath(tableName), Encoding.UTF8, TranslateUtils.JsonSerialize(tableInfo));
            }

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
