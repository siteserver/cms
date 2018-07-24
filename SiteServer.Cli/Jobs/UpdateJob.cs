using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.Cli.Jobs
{
    public static class UpdateJob
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

        public static async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(Options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (string.IsNullOrEmpty(_version))
            {
                await CliUtils.PrintErrorAsync("Error, the {version} to update is empty");
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                await CliUtils.PrintErrorAsync("Error, the update {directory} name is empty");
                return;
            }

            var oldTreeInfo = new TreeInfo(_directory);
            var newTreeInfo = new TreeInfo(Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                await CliUtils.PrintErrorAsync($"Error, directory {oldTreeInfo.DirectoryPath} not exists");
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
                await Console.Out.WriteLineAsync($"Error, the currently supported update versions are {Updater36.Version},{Updater40.Version},{Updater41.Version},{Updater50.Version}");
                return;
            }

            var newVersion = "latest";

            await Console.Out.WriteLineAsync($"Old Version: {_version}, Old Directory: {oldTreeInfo.DirectoryPath}");
            await Console.Out.WriteLineAsync($"New Version: {newVersion}, New Directory: {newTreeInfo.DirectoryPath}");

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(oldTreeInfo.TablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("Old Table Name", "New Table Name", "Total Count");
            await CliUtils.PrintRowLineAsync();

            var contentTableNameList = new List<string>();
            UpdateUtils.LoadContentTableNameList(oldTreeInfo, "siteserver_PublishmentSystem",
                contentTableNameList);
            UpdateUtils.LoadContentTableNameList(oldTreeInfo, "wcm_PublishmentSystem",
                contentTableNameList);

            foreach (var oldTableName in oldTableNames)
            {
                var oldMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldTableName);

                if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(await FileUtils.ReadTextAsync(oldMetadataFilePath, Encoding.UTF8));

                var tuple = await updater.UpdateTableInfoAsync(oldTableName, oldTableInfo, contentTableNameList);
                if (tuple != null)
                {
                    newTableNames.Add(tuple.Item1);

                    await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), Encoding.UTF8, TranslateUtils.JsonSerialize(tuple.Item2));
                }
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync("Well done! Thanks for Using SiteServer Cli Tool");
        }
    }
}
