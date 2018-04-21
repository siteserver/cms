using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class UpdateManager
    {
        public const string CommandName = "update";
        public const string Folder = "update";

        private static bool _isHelp;
        private static string _folderName = "backup";
        private static string _version;

        private static readonly OptionSet Options = new OptionSet() {
            { "f|folder=", "the backup {folder} name.",
                v => _folderName = !string.IsNullOrEmpty(v) ? v : "backup" },
            { "v|version=", "the {version} to update.",
                v => _version = v },
            { "h|help",  "show this message and exit",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("Update command usage: ");
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

            if (string.IsNullOrEmpty(_version))
            {
                Console.WriteLine("Error, Please input the version to update");
                return;
            }

            UpdaterBase updater = null;
            if (_version == Updater364.Version)
            {
                updater = new Updater364(_folderName, Folder);
            }
            if (updater == null)
            {
                Console.WriteLine($"Error, The currently supported update versions are {Updater364.Version}");
                return;
            }

            var newVersion = "latest";

            Console.WriteLine($"Old Version: {_version}");
            Console.WriteLine($"New Version: {newVersion}");

            if (!DirectoryUtils.IsDirectoryExists(CliUtils.GetBackupDirectoryPath(_folderName)))
            {
                Console.WriteLine("Error, Directory Not Exists");
                return;
            }

            var oldTablesFilePath = CliUtils.GetTablesFilePath(_folderName);
            if (!FileUtils.IsFileExists(oldTablesFilePath))
            {
                Console.WriteLine("Error, File _tables.json Not Exists");
                return;
            }

            DirectoryUtils.DeleteDirectoryIfExists(CliUtils.GetBackupDirectoryPath(Folder));

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(FileUtils.ReadText(oldTablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            CliUtils.PrintLine();
            CliUtils.PrintRow("Old Table Name", "New Table Name", "Total Count");
            CliUtils.PrintLine();

            foreach (var oldTableName in oldTableNames)
            {
                var oldMetadataFilePath = CliUtils.GetTableMetadataFilePath(_folderName, oldTableName);

                if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(oldMetadataFilePath, Encoding.UTF8));

                var kvp = updater.UpdateTableInfo(oldTableName, oldTableInfo);
                var newTableName = kvp.Key;
                var newTableInfo = kvp.Value;

                CliUtils.PrintRow(oldTableName, newTableName, oldTableInfo.TotalCount.ToString("#,0"));

                newTableNames.Add(newTableName);

                FileUtils.WriteText(CliUtils.GetTableMetadataFilePath(Folder, newTableName), Encoding.UTF8, TranslateUtils.JsonSerialize(newTableInfo));

                //DataProvider.DatabaseDao.CreateSystemTable(tableName, tableInfo.Columns);

                //foreach (var rowFileName in tableInfo.RowFiles)
                //{
                //    var filePath = PathUtils.Combine(oldDbFilesDirectoryPath, rowFileName);

                //    var objects = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                //    DataProvider.DatabaseDao.SyncObjects(tableName, objects, tableInfo.Columns);
                //}


            }

            FileUtils.WriteText(CliUtils.GetTablesFilePath(Folder), Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            CliUtils.PrintLine();
            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
