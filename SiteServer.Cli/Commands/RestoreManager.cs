using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class RestoreManager
    {
        public const string CommandName = "restore";

        private static bool _isHelp;
        private static string _folderName = "backup";
        private static string _webConfigFileName = "Web.config";

        private static readonly OptionSet Options = new OptionSet() {
            { "c|config=", "the {web.config} file name.",
                v => _webConfigFileName = !string.IsNullOrEmpty(v) ? v : "Web.config" },
            { "f|folder=", "the restore {folder} name.",
                v => _folderName = !string.IsNullOrEmpty(v) ? v : "backup" },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Restore command usage: ");
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

            Console.WriteLine($"DatabaseType: {WebConfigUtils.DatabaseType.Value}");
            Console.WriteLine($"ConnectionString: {WebConfigUtils.ConnectionString}");

            if (!DirectoryUtils.IsDirectoryExists(CliUtils.GetBackupDirectoryPath(_folderName)))
            {
                Console.WriteLine("Error, Directory Not Exists");
                return;
            }

            var tablesFilePath = CliUtils.GetTablesFilePath(_folderName);
            if (!FileUtils.IsFileExists(tablesFilePath))
            {
                Console.WriteLine("Error, File _tables.json Not Exists");
                return;
            }
            
            var tableNames = TranslateUtils.JsonDeserialize<List<string>>(FileUtils.ReadText(tablesFilePath, Encoding.UTF8));

            CliUtils.PrintLine();
            CliUtils.PrintRow("Import Table Name", "Total Count");
            CliUtils.PrintLine();

            foreach (var tableName in tableNames)
            {
                var metadataFilePath = CliUtils.GetTableMetadataFilePath(_folderName, tableName);

                if (!FileUtils.IsFileExists(metadataFilePath)) continue;

                var tableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(metadataFilePath, Encoding.UTF8));

                CliUtils.PrintRow(tableName, tableInfo.TotalCount.ToString("#,0"));

                if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    DataProvider.DatabaseDao.CreateSystemTable(tableName, tableInfo.Columns);
                }
                else
                {
                    DataProvider.DatabaseDao.AlterSystemTable(tableName, tableInfo.Columns);
                }

                foreach (var fileName in tableInfo.RowFiles)
                {
                    var objects = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(CliUtils.GetTableContentFilePath(_folderName, tableName, fileName), Encoding.UTF8));
                    DataProvider.DatabaseDao.SyncJObjects(tableName, objects, tableInfo.Columns);
                }

                //foreach (var item in objects)
                //{
                //    var dict = TranslateUtils.JsonGetDictionaryIgnorecase(item);

                //    foreach (var tableColumn in tableColumns)
                //    {
                //        object val;
                //        dict.TryGetValue(tableColumn.ColumnName, out val);

                //        if (tableColumn.DataType == DataType.Integer)
                //        {
                //            if (val == null) val = 0;
                //        }
                //        else if (tableColumn.DataType == DataType.Decimal)
                //        {
                //            if (val == null) val = 0;
                //        }
                //        else if (tableColumn.DataType == DataType.Boolean)
                //        {
                //            if (val == null) val = false;
                //        }
                //        else if (tableColumn.DataType == DataType.DateTime)
                //        {
                //            if (val == null) val = DateTime.Now;
                //        }

                //        Console.WriteLine($"name: {tableColumn.ColumnName}, type: {tableColumn.DataType}, value: {val}");
                //    }

                //    return;
                //}

            }

            CliUtils.PrintLine();
            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
