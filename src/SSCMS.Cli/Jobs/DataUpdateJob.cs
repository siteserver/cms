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
using SSCMS.Dto;

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

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: update database to latest schema");
            await console.WriteLineAsync("Options:");
            _options.WriteOptionDescriptions(console.Out);
            await console.WriteLineAsync();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            using var console = new ConsoleUtils(false);
            if (_isHelp)
            {
                await WriteUsageAsync(console);
                return;
            }

            if (string.IsNullOrEmpty(_directory))
            {
                await console.WriteErrorAsync("Backup folder name not specified: --directory");
                return;
            }

            var oldTree = new Tree(_settingsManager, _directory);
            var newTree = new Tree(_settingsManager, Folder);

            if (!DirectoryUtils.IsDirectoryExists(oldTree.DirectoryPath))
            {
                await console.WriteErrorAsync($"The backup folder does not exist: {oldTree.DirectoryPath}");
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(newTree.DirectoryPath);

            _updateService.Load(oldTree, newTree);

            await console.WriteLineAsync($"Backup folder: {oldTree.DirectoryPath}, Update folder: {newTree.DirectoryPath}, Update to SSCMS version: {_settingsManager.Version}");

            var oldTableNames = TranslateUtils.JsonDeserialize<List<string>>(await FileUtils.ReadTextAsync(oldTree.TablesFilePath, Encoding.UTF8));
            var newTableNames = new List<string>();

            await console.WriteRowLineAsync();
            await console.WriteRowAsync("Backup table name", "Update table Name", "Count");
            await console.WriteRowLineAsync();

            var siteIdList = new List<int>();
            var tableNames = new List<string>();

            UpdateUtils.LoadSites(_settingsManager, oldTree, siteIdList, tableNames);

            var table = new TableContentConverter(_settingsManager);

            var splitSiteTableDict = new Dictionary<int, Table>();
            if (_splitContents)
            {
                var converter = table.GetSplitConverter();
                foreach (var siteId in siteIdList)
                {
                    splitSiteTableDict.Add(siteId, new Table
                    {
                        Columns = converter.NewColumns,
                        TotalCount = 0,
                        Rows = new List<string>()
                    });
                }
            }

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
            var errorTableNames = new List<string>();

            foreach (var oldTableName in oldTableNames)
            {
                try
                {
                    var oldMetadataFilePath = oldTree.GetTableMetadataFilePath(oldTableName);

                    if (!FileUtils.IsFileExists(oldMetadataFilePath)) continue;

                    var oldTable = TranslateUtils.JsonDeserialize<Table>(await FileUtils.ReadTextAsync(oldMetadataFilePath, Encoding.UTF8));

                    if (ListUtils.ContainsIgnoreCase(tableNames, oldTableName))
                    {
                        if (_splitContents)
                        {
                            var converter = table.GetConverter(oldTableName, oldTable.Columns);

                            await _updateService.UpdateSplitContentsTableAsync(console, splitSiteTableDict, siteIdList, oldTableName,
                                oldTable, converter);
                        }
                        else
                        {
                            var converter = table.GetConverter(oldTableName, oldTable.Columns);
                            var tuple = await _updateService.GetNewTableAsync(console, oldTableName, oldTable, converter);
                            if (tuple != null)
                            {
                                newTableNames.Add(tuple.Item1);

                                await FileUtils.WriteTextAsync(newTree.GetTableMetadataFilePath(tuple.Item1), TranslateUtils.JsonSerialize(tuple.Item2));
                            }
                        }
                    }
                    else
                    {
                        var tuple = await _updateService.UpdateTableAsync(console, oldTableName, oldTable);
                        if (tuple != null)
                        {
                            newTableNames.Add(tuple.Item1);

                            await FileUtils.WriteTextAsync(newTree.GetTableMetadataFilePath(tuple.Item1), TranslateUtils.JsonSerialize(tuple.Item2));
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorTableNames.Add(oldTableName);
                    await FileUtils.AppendErrorLogAsync(errorLogFilePath, new TextLog
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
                    var siteTable = splitSiteTableDict[siteId];
                    var siteTableName = UpdateUtils.GetSplitContentTableName(siteId);
                    newTableNames.Add(siteTableName);

                    await FileUtils.WriteTextAsync(newTree.GetTableMetadataFilePath(siteTableName), TranslateUtils.JsonSerialize(siteTable));
                }

                await UpdateUtils.UpdateSitesSplitTableNameAsync(_databaseManager, newTree, splitSiteTableDict);
            }

            await FileUtils.WriteTextAsync(newTree.TablesFilePath, TranslateUtils.JsonSerialize(newTableNames));

            await console.WriteRowLineAsync();
            if (errorTableNames.Count == 0)
            {
                await console.WriteSuccessAsync("Update the backup data to the new version successfully!");
            }
            else
            {
                await console.WriteErrorAsync($"Database update failed and the following table was not successfully update: {ListUtils.ToString(errorTableNames)}");
            }
        }
    }
}
