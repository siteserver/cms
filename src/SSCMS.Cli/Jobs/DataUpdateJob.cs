using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Cli.Updater;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataUpdateJob : IJobService
    {
        public string CommandName => "data update";
        private const string Folder = "update";

        private string _directory;
        private bool _splitContents;
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
                { "d|directory=", "Backup folder name",
                    v => _directory = v },
                { "split-contents",  "Split content table by site",
                    v => _splitContents = v != null },
                { "h|help",  "Display help",
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

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                await WriteUtils.PrintErrorAsync("Backup folder name not specified: --directory");
                return;
            }

            var oldTreeInfo = new TreeInfo(_settingsManager, _directory);
            var newTreeInfo = new TreeInfo(_settingsManager, Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTreeInfo.DirectoryPath))
            {
                await WriteUtils.PrintErrorAsync($"The backup folder does not exist: {oldTreeInfo.DirectoryPath}");
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(newTreeInfo.DirectoryPath);

            _updateService.Load(oldTreeInfo, newTreeInfo);

            await Console.Out.WriteLineAsync($"Backup folder: {oldTreeInfo.DirectoryPath}, Update folder: {newTreeInfo.DirectoryPath}, Update to SSCMS version: {_settingsManager.Version}");

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(oldTreeInfo.TablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            await WriteUtils.PrintRowLineAsync();
            await WriteUtils.PrintRowAsync("Backup table name", "Update table Name", "Count");
            await WriteUtils.PrintRowLineAsync();

            var siteIdList = new List<int>();
            var tableNames = new List<string>();

            UpdateUtils.LoadSites(_settingsManager, oldTreeInfo, siteIdList, tableNames);

            var table = new TableContentConverter(_settingsManager);

            var splitSiteTableDict = new Dictionary<int, TableInfo>();
            if (_splitContents)
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

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
            var errorTableNames = new List<string>();

            foreach (var oldTableName in oldTableNames)
            {
                try
                {
                    var oldMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldTableName);

                    if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                    var oldTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(await FileUtils.ReadTextAsync(oldMetadataFilePath, Encoding.UTF8));

                    if (ListUtils.ContainsIgnoreCase(tableNames, oldTableName))
                    {
                        if (_splitContents)
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
                        var tuple = await _updateService.UpdateTableInfoAsync(oldTableName, oldTableInfo);
                        if (tuple != null)
                        {
                            newTableNames.Add(tuple.Item1);

                            await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(tuple.Item1), TranslateUtils.JsonSerialize(tuple.Item2));
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorTableNames.Add(oldTableName);
                    await CliUtils.AppendErrorLogAsync(errorLogFilePath, new TextLogInfo
                    {
                        Exception = ex,
                        DateTime = DateTime.Now,
                        Detail = oldTableName
                    });
                }
            }

            if (_splitContents)
            {
                foreach (var siteId in siteIdList)
                {
                    var siteTableInfo = splitSiteTableDict[siteId];
                    var siteTableName = UpdateUtils.GetSplitContentTableName(siteId);
                    newTableNames.Add(siteTableName);

                    await FileUtils.WriteTextAsync(newTreeInfo.GetTableMetadataFilePath(siteTableName), TranslateUtils.JsonSerialize(siteTableInfo));
                }

                await UpdateUtils.UpdateSitesSplitTableNameAsync(_databaseManager, newTreeInfo, splitSiteTableDict);
            }

            await FileUtils.WriteTextAsync(newTreeInfo.TablesFilePath, TranslateUtils.JsonSerialize(newTableNames));

            await WriteUtils.PrintRowLineAsync();
            if (errorTableNames.Count == 0)
            {
                await WriteUtils.PrintSuccessAsync("Update the backup data to the new version successfully!");
            }
            else
            {
                await WriteUtils.PrintErrorAsync($"Database update failed and the following table was not successfully update: {ListUtils.ToString(errorTableNames)}");
            }
        }
    }
}
