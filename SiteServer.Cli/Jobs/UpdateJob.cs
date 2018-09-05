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
            { "d|directory=", "指定需要转换至最新版本的备份数据文件夹",
                v => _directory = v },
            { "v|version=", "指定需要转换的备份数据版本号",
                v => _version = v },
            { "h|help",  "命令说明",
                v => _isHelp = v != null }
        };

        public static void PrintUsage()
        {
            Console.WriteLine("升级备份数据: siteserver update");
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
                await CliUtils.PrintErrorAsync("未指定需要转换的备份数据版本号： version");
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                await CliUtils.PrintErrorAsync("未指定需要转换至最新版本的备份数据文件夹：directory");
                return;
            }

            var oldTreeInfo = new TreeInfo(_directory);
            var newTreeInfo = new TreeInfo(Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                await CliUtils.PrintErrorAsync($"备份数据的文件夹 {oldTreeInfo.DirectoryPath} 不存在");
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
                await Console.Out.WriteLineAsync($"当前支持的升级版本号必须是 {Updater36.Version},{Updater40.Version},{Updater41.Version},{Updater50.Version} 中的一项");
                return;
            }

            var newVersion = "latest";

            await Console.Out.WriteLineAsync($"备份版本: {_version}, 备份数据文件夹: {oldTreeInfo.DirectoryPath}");
            await Console.Out.WriteLineAsync($"升级版本: {newVersion}, 升级数据文件夹: {newTreeInfo.DirectoryPath}");

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(oldTreeInfo.TablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            await CliUtils.PrintRowLineAsync();
            await CliUtils.PrintRowAsync("备份表名称", "升级表名称", "总条数");
            await CliUtils.PrintRowLineAsync();

            var tableNameListForContent = new List<string>();
            var tableNameListForGovPublic = new List<string>();
            var tableNameListForGovInteract = new List<string>();
            var tableNameListForJob = new List<string>();

            UpdateUtils.LoadContentTableNameList(oldTreeInfo, "siteserver_PublishmentSystem",
                tableNameListForContent, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);
            UpdateUtils.LoadContentTableNameList(oldTreeInfo, "wcm_PublishmentSystem",
                tableNameListForContent, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);

            foreach (var oldTableName in oldTableNames)
            {
                var oldMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldTableName);

                if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(await FileUtils.ReadTextAsync(oldMetadataFilePath, Encoding.UTF8));

                var tuple = await updater.UpdateTableInfoAsync(oldTableName, oldTableInfo, tableNameListForContent, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);
                if (tuple != null)
                {
                    newTableNames.Add(tuple.Item1);

                    await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), Encoding.UTF8, TranslateUtils.JsonSerialize(tuple.Item2));
                }
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功从备份文件夹：{oldTreeInfo.DirectoryPath} 升级至新版本：{newTreeInfo.DirectoryPath} ！");
        }
    }
}
