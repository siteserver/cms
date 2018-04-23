using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater;
using SiteServer.Cli.Updater.Model36;
using SiteServer.Utils;

namespace SiteServer.Cli.Commands
{
    public static class UpdateManager
    {
        public const string CommandName = "update";
        private const string Folder = "update";

        private static bool _isHelp;
        private static string _directory;
        private static string _version;

        private static readonly OptionSet Options = new OptionSet() {
            { "d|directory=", "the update {directory} name.",
                v => _directory = v },
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

            if (string.IsNullOrEmpty(_directory))
            {
                _directory = $"backup/{DateTime.Now:yyyy-MM-dd}";
            }

            var oldTreeInfo = new TreeInfo(_directory);
            var newTreeInfo = new TreeInfo(Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                CliUtils.PrintError($"Error, Directory {oldTreeInfo.DirectoryPath} Not Exists");
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(newTreeInfo.DirectoryPath);

            UpdaterBase updater = null;
            if (_version == Updater36.Version)
            {
                updater = new Updater36(oldTreeInfo, newTreeInfo);
            }
            if (updater == null)
            {
                Console.WriteLine($"Error, The currently supported update versions are {Updater36.Version}");
                return;
            }

            var newVersion = "latest";

            Console.WriteLine($"Old Version: {_version}, Old Directory: {oldTreeInfo.DirectoryPath}");
            Console.WriteLine($"New Version: {newVersion}, New Directory: {newTreeInfo.DirectoryPath}");

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(FileUtils.ReadText(oldTreeInfo.TablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            CliUtils.PrintLine();
            CliUtils.PrintRow("Old Table Name", "New Table Name", "Total Count");
            CliUtils.PrintLine();

            var contentTableNameList = new List<string>();
            var siteMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath("siteserver_PublishmentSystem");
            if (FileUtils.IsFileExists(siteMetadataFilePath))
            {
                var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(siteMetadataFilePath, Encoding.UTF8));
                foreach (var fileName in siteTableInfo.RowFiles)
                {
                    var filePath = oldTreeInfo.GetTableContentFilePath("siteserver_PublishmentSystem", fileName);
                    var rows = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                    foreach (var row in rows)
                    {
                        var dict = TranslateUtils.JsonGetDictionaryIgnorecase(row);
                        object obj;
                        if (dict.TryGetValue(nameof(SiteserverPublishmentSystem.AuxiliaryTableForContent),
                            out obj))
                        {
                            if (obj != null)
                            {
                                contentTableNameList.Add(obj.ToString());
                            }
                        }
                    }
                }
            }

            foreach (var oldTableName in oldTableNames)
            {
                var oldMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldTableName);

                if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(oldMetadataFilePath, Encoding.UTF8));

                var kvp = updater.UpdateTableInfo(oldTableName, oldTableInfo, contentTableNameList);
                var newTableName = kvp.Key;
                var newTableInfo = kvp.Value;

                CliUtils.PrintRow(oldTableName, newTableName, oldTableInfo.TotalCount.ToString("#,0"));

                newTableNames.Add(newTableName);

                FileUtils.WriteText(newTreeInfo.GetTableMetadataFilePath(newTableName), Encoding.UTF8, TranslateUtils.JsonSerialize(newTableInfo));

                //DataProvider.DatabaseDao.CreateSystemTable(tableName, tableInfo.Columns);

                //foreach (var rowFileName in tableInfo.RowFiles)
                //{
                //    var filePath = PathUtils.Combine(oldDbFilesDirectoryPath, rowFileName);

                //    var objects = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                //    DataProvider.DatabaseDao.SyncObjects(tableName, objects, tableInfo.Columns);
                //}


            }

            FileUtils.WriteText(newTreeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            CliUtils.PrintLine();
            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
