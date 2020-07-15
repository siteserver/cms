using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Cli.Updater;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataUpdateJob : IJobService
    {
        public string CommandName => "data update";
        private const string Folder = "update";

        private string _directory;
        private bool _contentSplit;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IDataUpdateService _updateService;
        private readonly OptionSet _options;

        public DataUpdateJob(ISettingsManager settingsManager, IDatabaseManager databaseManager, IDataUpdateService updateService)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _updateService = updateService;

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
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: update database to latest schema");
            Console.WriteLine("Options:");
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
                await WriteUtils.PrintErrorAsync("未指定需要转换至最新版本的备份数据文件夹：directory");
                return;
            }

            var oldTreeInfo = new TreeInfo(_settingsManager, _directory);
            var newTreeInfo = new TreeInfo(_settingsManager, Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                await WriteUtils.PrintErrorAsync($"备份数据的文件夹 {oldTreeInfo.DirectoryPath} 不存在");
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(newTreeInfo.DirectoryPath);

            _updateService.Load(oldTreeInfo, newTreeInfo);

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"备份数据文件夹: {oldTreeInfo.DirectoryPath}，升级数据文件夹: {newTreeInfo.DirectoryPath}，升级版本: {version.Substring(0, version.Length - 2)}");

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(oldTreeInfo.TablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintRowAsync("备份表名称", "升级表名称", "总条数");
            await WriteUtils.PrintRowLineAsync();

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

                if (ListUtils.ContainsIgnoreCase(tableNameListForContent, oldTableName))
                {
                    if (_contentSplit)
                    {
                        var converter = table.GetConverter(oldTableName, oldTableInfo.Columns);

                        await _updateService.UpdateSplitContentsTableInfoAsync(splitSiteTableDict, siteIdList, oldTableName,
                            oldTableInfo, converter);
                    }
                    else
                    {
                        var converter = table.GetConverter(oldTableName, oldTableInfo.Columns);
                        var tuple = await _updateService.GetNewTableInfoAsync(oldTableName, oldTableInfo, converter);
                        if (tuple != null)
                        {
                            newTableNames.Add(tuple.Item1);

                            await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), TranslateUtils.JsonSerialize(tuple.Item2));
                        }
                    }
                }
                else
                {
                    var tuple = await _updateService.UpdateTableInfoAsync(oldTableName, oldTableInfo, tableNameListForGovPublic, tableNameListForGovInteract, tableNameListForJob);
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
                    var siteTableName = await _databaseManager.ContentRepository.GetNewContentTableNameAsync();
                    newTableNames.Add(siteTableName);

                    await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(siteTableName), TranslateUtils.JsonSerialize(siteTableInfo));
                }

                await UpdateUtils.UpdateSitesSplitTableNameAsync(_databaseManager, newTreeInfo, splitSiteTableDict);
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, TranslateUtils.JsonSerialize(newTableNames));

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintSuccessAsync($"恭喜，成功从备份文件夹：{oldTreeInfo.DirectoryPath} 升级至新版本：{newTreeInfo.DirectoryPath} ！");
        }
    }
}
