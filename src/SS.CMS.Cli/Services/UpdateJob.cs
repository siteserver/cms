using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using SS.CMS.Abstractions;
using SS.CMS.Cli.Core;
using SS.CMS.Cli.Updater;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Services
{
    public class UpdateJob : IJobService
    {
        public string CommandName => "update";
        private const string Folder = "update";

        private string _directory;
        private bool _contentSplit;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly UpdaterManager _updaterManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly OptionSet _options;

        public UpdateJob(ISettingsManager settingsManager, UpdaterManager updaterManager, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _updaterManager = updaterManager;
            _databaseManager = databaseManager;
            _options = new OptionSet {
                { "d|directory=", "指定需要升级至最新版本的备份数据文件夹",
                    v => _directory = v },
                { "content-split",  "拆分内容表",
                    v => _contentSplit = v != null },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine("系统升级: siteserver update");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

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

            var oldTreeInfo = new TreeInfo(_settingsManager, _directory);
            var newTreeInfo = new TreeInfo(_settingsManager, Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                await CliUtils.PrintErrorAsync($"备份数据的文件夹 {oldTreeInfo.DirectoryPath} 不存在");
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(newTreeInfo.DirectoryPath);

            _updaterManager.Load(oldTreeInfo, newTreeInfo);

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

            var table = new TableContentConverter(_settingsManager);

            var splitSiteTableDict = new Dictionary<int, TableInfo>();
            if (_contentSplit)
            {
                var converter = table.GetSplitConverter();
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
                        var converter = table.GetConverter(oldTableName, oldTableInfo.Columns);

                        await _updaterManager.UpdateSplitContentsTableInfoAsync(splitSiteTableDict, siteIdList, oldTableName,
                            oldTableInfo, converter);
                    }
                    else
                    {
                        var converter = table.GetConverter(oldTableName, oldTableInfo.Columns);
                        var tuple = await _updaterManager.GetNewTableInfoAsync(oldTableName, oldTableInfo, converter);
                        if (tuple != null)
                        {
                            newTableNames.Add(tuple.Item1);

                            await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), TranslateUtils.JsonSerialize(tuple.Item2));
                        }
                    }
                }
                else
                {
                    var tuple = await _updaterManager.UpdateTableInfoAsync(oldTableName, oldTableInfo, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);
                    if (tuple != null)
                    {
                        newTableNames.Add(tuple.Item1);

                        await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), TranslateUtils.JsonSerialize(tuple.Item2));
                    }
                }
            }

            if (_contentSplit)
            {
                foreach (var siteId in siteIdList)
                {
                    var siteTableInfo = splitSiteTableDict[siteId];
                    var siteTableName = ContentRepository.GetContentTableName(siteId);
                    newTableNames.Add(siteTableName);

                    await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(siteTableName), TranslateUtils.JsonSerialize(siteTableInfo));
                }

                await UpdateUtils.UpdateSitesSplitTableNameAsync(_databaseManager, newTreeInfo, splitSiteTableDict);
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, TranslateUtils.JsonSerialize(newTableNames));

            await CliUtils.PrintRowLineAsync();
            await Console.Out.WriteLineAsync($"恭喜，成功从备份文件夹：{oldTreeInfo.DirectoryPath} 升级至新版本：{newTreeInfo.DirectoryPath} ！");
        }
    }
}
