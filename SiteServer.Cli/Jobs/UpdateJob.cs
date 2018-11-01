using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater;
using SiteServer.CMS.Core;
using SiteServer.CMS.Provider;
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
        private static bool _contentSplit;

        private static readonly OptionSet Options = new OptionSet() {
            { "d|directory=", "指定需要转换至最新版本的备份数据文件夹",
                v => _directory = v },
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

            var updater = new UpdaterManager(oldTreeInfo, newTreeInfo);

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"备份数据文件夹: {oldTreeInfo.DirectoryPath}，升级数据文件夹: {newTreeInfo.DirectoryPath}，升级版本: {version.Substring(0, version.Length - 2)}");

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

            UpdateUtils.LoadSites(oldTreeInfo, siteIdList,
                tableNameListForContent, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);

            var splitSiteTableDict = new Dictionary<int, TableInfo>();
            if (_contentSplit)
            {
                var converter = ContentConverter.GetSplitConverter();
                foreach (var siteId in siteIdList)
                {
                    splitSiteTableDict.Add(siteId, new TableInfo
                    {
                        Columns = converter.NewColumns,
                        TotalCount = 0,
                        RowFiles = new List<string>()
                    });
                }
            }

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

                        await updater.UpdateSplitContentsTableInfoAsync(splitSiteTableDict, siteIdList, oldTableName,
                            oldTableInfo, converter);
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
                foreach (var siteId in siteIdList)
                {
                    var siteTableInfo = splitSiteTableDict[siteId];
                    var siteTableName = ContentDao.GetContentTableName(siteId);
                    newTableNames.Add(siteTableName);

                    await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(siteTableName), Encoding.UTF8, TranslateUtils.JsonSerialize(siteTableInfo));
                }

                await UpdateUtils.UpdateSitesSplitTableNameAsync(newTreeInfo, splitSiteTableDict);
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newTableNames));

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功从备份文件夹：{oldTreeInfo.DirectoryPath} 升级至新版本：{newTreeInfo.DirectoryPath} ！");
        }
    }
}
