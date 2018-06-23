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
            Console.WriteLine("Update command usage: siteserver update");
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
                Console.WriteLine("Error, the {version} to update is empty");
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                CliUtils.PrintError("Error, the update {directory} name is empty");
                return;
            }

            var oldTreeInfo = new TreeInfo(_directory);
            var newTreeInfo = new TreeInfo(Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                CliUtils.PrintError($"Error, directory {oldTreeInfo.DirectoryPath} not exists");
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(newTreeInfo.DirectoryPath);

            UpdaterBase updater = null;

            if (_version == Updater36.Version)
            {
                updater = new Updater36(oldTreeInfo, newTreeInfo);
            }
            else if (_version == Updater40.Version)
            {
                updater = new Updater40(oldTreeInfo, newTreeInfo);
            }
            else if (_version == Updater41.Version)
            {
                updater = new Updater41(oldTreeInfo, newTreeInfo);
            }
            else if (_version == Updater50.Version)
            {
                updater = new Updater50(oldTreeInfo, newTreeInfo);
            }
            if (updater == null)
            {
                Console.WriteLine($"Error, the currently supported update versions are {Updater36.Version},{Updater40.Version},{Updater41.Version},{Updater50.Version}");
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
            UpdateUtils.LoadContentTableNameList(oldTreeInfo, "siteserver_PublishmentSystem",
                contentTableNameList);
            UpdateUtils.LoadContentTableNameList(oldTreeInfo, "wcm_PublishmentSystem",
                contentTableNameList);

            foreach (var oldTableName in oldTableNames)
            {
                var oldMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldTableName);

                if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(oldMetadataFilePath, Encoding.UTF8));

                string newTableName;
                TableInfo newTableInfo;
                if (updater.UpdateTableInfo(oldTableName, oldTableInfo, contentTableNameList, out newTableName, out newTableInfo))
                {
                    newTableNames.Add(newTableName);

                    FileUtils.WriteText(newTreeInfo.GetTableMetadataFilePath(newTableName), Encoding.UTF8, TranslateUtils.JsonSerialize(newTableInfo));
                }
            }

            FileUtils.WriteText(newTreeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            CliUtils.PrintLine();
            Console.WriteLine("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
