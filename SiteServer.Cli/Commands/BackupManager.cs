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
        private const int PageSize = 1000;

        private static bool _isHelp;
        private static string _folderName = "backup";
        private static string _webConfigFileName = "Web.config";

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {web.config} file name.",
                v => _webConfigFileName = !string.IsNullOrEmpty(v) ? v : "Web.config" },
            { "f|folder=", "the backup {folder} name.",
                v => _folderName = !string.IsNullOrEmpty(v) ? v : "backup" },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Backup command usage: ");
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

            WebConfigUtils.Load(CliUtils.PhysicalApplicationPath, _webConfigFileName);

            DirectoryUtils.DeleteDirectoryIfExists(CliUtils.GetBackupDirectoryPath(_folderName));

            Console.WriteLine($"DatabaseType: {WebConfigUtils.DatabaseType.Value}");
            Console.WriteLine($"ConnectionString: {WebConfigUtils.ConnectionString}");

            var tableNames = DataProvider.DatabaseDao.GetTableNameList();

            FileUtils.WriteText(CliUtils.GetTablesFilePath(_folderName), Encoding.UTF8, TranslateUtils.JsonSerialize(tableNames));

            CliUtils.PrintLine();
            CliUtils.PrintRow("Backup Table Name", "Total Count");
            CliUtils.PrintLine();

            foreach (var tableName in tableNames)
            {
                var tableInfo = new TableInfo
                {
                    Columns = DataProvider.DatabaseDao.GetTableColumnInfoListLowercase(WebConfigUtils.ConnectionString, tableName),
                    TotalCount = DataProvider.DatabaseDao.GetCount(tableName),
                    RowFiles = new List<string>()
                };

                CliUtils.PrintRow(tableName, tableInfo.TotalCount.ToString("#,0"));

                var identityColumnName = DataProvider.DatabaseDao.AddIdentityColumnIdIfNotExists(tableName, tableInfo.Columns);

                if (tableInfo.TotalCount > 0)
                {
                    var current = 1;
                    if (tableInfo.TotalCount > PageSize)
                    {
                        var pageCount = (int)Math.Ceiling((double)tableInfo.TotalCount / PageSize);

                        for (; current <= pageCount; current++)
                        {
                            var fileName = $"{current}.json";
                            tableInfo.RowFiles.Add(fileName);
                            var offset = (current - 1) * PageSize;
                            var limit = PageSize;

                            var rows = DataProvider.DatabaseDao.GetPageObjects(tableName, identityColumnName, offset, limit);

                            FileUtils.WriteText(CliUtils.GetTableContentFilePath(_folderName, tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);
                        var rows = DataProvider.DatabaseDao.GetObjects(tableName);

                        FileUtils.WriteText(CliUtils.GetTableContentFilePath(_folderName, tableName, fileName), Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                    }
                }

                FileUtils.WriteText(CliUtils.GetTableMetadataFilePath(_folderName, tableName), Encoding.UTF8, TranslateUtils.JsonSerialize(tableInfo));
            }

            CliUtils.PrintLine();
            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
