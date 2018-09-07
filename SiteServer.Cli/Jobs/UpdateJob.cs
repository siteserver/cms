using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater;
using SiteServer.CMS.Core;
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
        private static bool _contentSplit;

        private static readonly OptionSet Options = new OptionSet() {
            { "d|directory=", "指定需要转换至最新版本的备份数据文件夹",
                v => _directory = v },
            { "v|version=", "指定需要转换的备份数据版本号",
                v => _version = v },
            { "content-split",  "拆分内容表",
                v => _contentSplit = v != null },
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

            if (_version == Updater3.Version)
            {
                updater = new Updater3(oldTreeInfo, newTreeInfo);
            }
            else if (_version == Updater4.Version)
            {
                updater = new Updater4(oldTreeInfo, newTreeInfo);
            }
            else if (_version == Updater5.Version)
            {
                updater = new Updater5(oldTreeInfo, newTreeInfo);
            }
            if (updater == null)
            {
                await Console.Out.WriteLineAsync($"当前支持的升级版本号必须是 {Updater3.Version},{Updater4.Version},{Updater5.Version} 中的一项");
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

            var siteIdList = new List<int>();
            var tableNameListForContent = new List<string>();
            var tableNameListForGovPublic = new List<string>();
            var tableNameListForGovInteract = new List<string>();
            var tableNameListForJob = new List<string>();

            UpdateUtils.LoadSites(oldTreeInfo, "siteserver_PublishmentSystem", siteIdList,
                tableNameListForContent, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);
            UpdateUtils.LoadSites(oldTreeInfo, "wcm_PublishmentSystem", siteIdList,
                tableNameListForContent, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);

            foreach (var oldTableName in oldTableNames)
            {
                var oldMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldTableName);

                if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(await FileUtils.ReadTextAsync(oldMetadataFilePath, Encoding.UTF8));

                if (StringUtils.ContainsIgnoreCase(tableNameListForContent, oldTableName))
                {
                    if (_contentSplit)
                    {
                        var converter = ContentConverter.GetConverter(oldTableName, oldTableInfo.Columns);

                        var tupleList = await updater.GetNewSplitContentsTableInfoAsync(siteIdList, oldTableName,
                            oldTableInfo, converter);

                        foreach (var tuple in tupleList)
                        {
                            newTableNames.Add(tuple.Item2);

                            await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item2), Encoding.UTF8, TranslateUtils.JsonSerialize(tuple.Item3));
                        }
                    }
                    else
                    {
                        var converter = ContentConverter.GetConverter(oldTableName, oldTableInfo.Columns);
                        var tuple = await updater.GetNewTableInfoAsync(oldTableName, oldTableInfo, converter);
                        if (tuple != null)
                        {
                            newTableNames.Add(tuple.Item1);

                            await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), Encoding.UTF8, TranslateUtils.JsonSerialize(tuple.Item2));
                        }
                    }
                }
                else
                {
                    var tuple = await updater.UpdateTableInfoAsync(oldTableName, oldTableInfo, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);
                    if (tuple != null)
                    {
                        newTableNames.Add(tuple.Item1);

                        await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), Encoding.UTF8, TranslateUtils.JsonSerialize(tuple.Item2));
                    }
                }
            }

            if (_contentSplit)
            {
                await UpdateUtils.UpdateSitesSplitTableNameAsync(newTreeInfo, DataProvider.SiteDao.TableName);
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功从备份文件夹：{oldTreeInfo.DirectoryPath} 升级至新版本：{newTreeInfo.DirectoryPath} ！");
        }
    }
}
