using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class BackupManager
    {
        public const string CommandName = "backup";

        private static bool _isHelp;
        private static string _directory;
        private static string _configFileName;

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {cli.json} file name.",
                v => _configFileName = v },
            { "d|directory=", "the backup {directory} name.",
                v => _directory = v },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Backup command usage: siteserver backup");
            Options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public static void Execute(string[] args)
        {
            if (!CliUtils.ParseArgs(Options, args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                CliUtils.PrintError("Error, the backup {directory} name is empty");
                return;
            }

            var treeInfo = new TreeInfo(_directory);
            DirectoryUtils.CreateDirectoryIfNotExists(treeInfo.DirectoryPath);

            var configInfo = CliUtils.LoadConfig(_configFileName);
            if (configInfo == null)
            {
                CliUtils.PrintError("Error, config file cli.json not exists");
                return;
            }

            if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
            {
                CliUtils.PrintError("Error, connection string is empty");
                return;
            }

            var isConnectValid = DataProvider.DatabaseDao.ConnectToServer(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, out _, out _);
            if (!isConnectValid)
            {
                CliUtils.PrintError("Error, connection string not correct");
                return;
            }

            Console.WriteLine($"Database Type: {WebConfigUtils.DatabaseType.Value}");
            Console.WriteLine($"Connection String: {WebConfigUtils.ConnectionString}");
            Console.WriteLine($"Backup Directory: {treeInfo.DirectoryPath}");

            var tableNames = DataProvider.DatabaseDao.GetTableNameList();

            FileUtils.WriteText(treeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(tableNames));

            CliUtils.PrintLine();
            CliUtils.PrintRow("Backup Table Name", "Total Count");
            CliUtils.PrintLine();

            foreach (var tableName in tableNames)
            {
                if (configInfo.BackupConfig.Includes != null)
                {
                    if (!StringUtils.ContainsIgnoreCase(configInfo.BackupConfig.Includes, tableName)) continue;
                }
                if (configInfo.BackupConfig.Excludes != null)
                {
                    if (StringUtils.ContainsIgnoreCase(configInfo.BackupConfig.Includes, tableName)) continue;
                }

                var tableInfo = new TableInfo
                {
                    Columns = DataProvider.DatabaseDao.GetTableColumnInfoList(WebConfigUtils.ConnectionString, tableName),
                    TotalCount = DataProvider.DatabaseDao.GetCount(tableName),
                    RowFiles = new List<string>()
                };

                CliUtils.PrintRow(tableName, tableInfo.TotalCount.ToString("#,0"));

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

                                FileUtils.WriteText(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = DataProvider.DatabaseDao.GetObjects(tableName);

                        FileUtils.WriteText(treeInfo.GetTableContentFilePath(tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                    }
                }

                FileUtils.WriteText(treeInfo.GetTableMetadataFilePath(tableName), Encoding.UTF8, TranslateUtils.JsonSerialize(tableInfo));
            }

            CliUtils.PrintLine();
            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
