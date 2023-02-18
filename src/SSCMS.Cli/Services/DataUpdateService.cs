using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Cli.Updater;
using SSCMS.Cli.Updater.Tables;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Services
{
    public class DataUpdateService : IDataUpdateService
    {
        private readonly IDatabaseManager _databaseManager;

        public DataUpdateService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        protected Tree OldTree { get; private set; }

        protected Tree NewTree { get; private set; }

        public void Load(Tree oldTree, Tree newTree)
        {
            OldTree = oldTree;
            NewTree = newTree;
        }

        public async Task<Tuple<string, Table>> GetNewTableAsync(IConsoleUtils console, string oldTableName, Table oldTable, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await console.WriteRowAsync(oldTableName, "Abandon", "--");
                return null;
            }

            if (string.IsNullOrEmpty(converter.NewTableName))
            {
                converter.NewTableName = oldTableName;
            }
            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTable.Columns;
            }

            var newTable = new Table
            {
                Columns = converter.NewColumns,
                TotalCount = oldTable.TotalCount,
                Rows = oldTable.Rows
            };

            await console.WriteRowAsync(oldTableName, converter.NewTableName, oldTable.TotalCount.ToString("#,0"));

            if (oldTable.Rows.Count > 0)
            {
                using var progress = new ConsoleUtils(true);
                var i = 0;
                foreach (var fileName in oldTable.Rows)
                {
                    progress.Report((double)i++ / oldTable.Rows.Count);

                    var oldFilePath = OldTree.GetTableContentFilePath(oldTableName, fileName);
                    var newFilePath = NewTree.GetTableContentFilePath(converter.NewTableName, fileName);

                    if (converter.ConvertKeyDict != null)
                    {
                        var oldRows =
                            TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                        var newRows = UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict, converter.Process);

                        await FileUtils.WriteTextAsync(newFilePath, TranslateUtils.JsonSerialize(newRows));
                    }
                    else
                    {
                        FileUtils.CopyFile(oldFilePath, newFilePath);
                    }
                }
            }

            return new Tuple<string, Table>(converter.NewTableName, newTable);
        }

        public async Task UpdateSplitContentsTableAsync(IConsoleUtils console, Dictionary<int, Table> splitSiteTableDict, List<int> siteIdList, string oldTableName, Table oldTable, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await console.WriteRowAsync(oldTableName, "Abandon", "--");
                return;
            }

            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTable.Columns;
            }

            await console.WriteRowAsync(oldTableName, "#split-contents#", oldTable.TotalCount.ToString("#,0"));

            if (oldTable.Rows.Count > 0)
            {
                using var progress = new ConsoleUtils(true);
                var i = 0;
                foreach (var fileName in oldTable.Rows)
                {
                    progress.Report((double)i++ / oldTable.Rows.Count);

                    var newRows = new List<Dictionary<string, object>>();

                    var oldFilePath = OldTree.GetTableContentFilePath(oldTableName, fileName);

                    var oldRows =
                        TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                    newRows.AddRange(UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict, converter.Process));

                    var siteIdWithRows = new Dictionary<int, List<Dictionary<string, object>>>();
                    foreach (var siteId in siteIdList)
                    {
                        siteIdWithRows.Add(siteId, new List<Dictionary<string, object>>());
                    }

                    foreach (var newRow in newRows)
                    {
                        if (newRow.ContainsKey(nameof(Content.SiteId)))
                        {
                            var siteId = Convert.ToInt32(newRow[nameof(Content.SiteId)]);
                            if (siteIdList.Contains(siteId))
                            {
                                var rows = siteIdWithRows[siteId];
                                rows.Add(newRow);
                            }
                        }
                    }

                    foreach (var siteId in siteIdList)
                    {
                        var siteRows = siteIdWithRows[siteId];
                        var siteTableName = UpdateUtils.GetSplitContentTableName(siteId);

                        var siteTable = splitSiteTableDict[siteId];
                        siteTable.TotalCount += siteRows.Count;

                        foreach(var tableColumn in converter.NewColumns)
                        {
                            if (!siteTable.Columns.Any(t => StringUtils.EqualsIgnoreCase(t.AttributeName, tableColumn.AttributeName)))
                            {
                                siteTable.Columns.Add(tableColumn);
                            }
                        }

                        if (siteRows.Count > 0)
                        {
                            var siteTableFileName = $"{siteTable.Rows.Count + 1}.json";
                            siteTable.Rows.Add(siteTableFileName);
                            var filePath = NewTree.GetTableContentFilePath(siteTableName, siteTableFileName);
                            await FileUtils.WriteTextAsync(filePath, TranslateUtils.JsonSerialize(siteRows));
                        }
                    }
                }
            }
        }

        public async Task<Tuple<string, Table>> UpdateTableAsync(IConsoleUtils console, string oldTableName, Table oldTable)
        {
            ConvertInfo converter = null;

            if (StringUtils.EqualsIgnoreCase(TableAdministrator.OldTableName, oldTableName))
            {
                var table = new TableAdministrator(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableAdministratorsInRoles.OldTableName, oldTableName))
            {
                var table = new TableAdministratorsInRoles(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableChannel.OldTableNames, oldTableName))
            {
                var table = new TableChannel(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableChannelGroup.OldTableNames, oldTableName))
            {
                var table = new TableChannelGroup(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableConfig.OldTableName, oldTableName))
            {
                var table = new TableConfig(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableContentCheck.OldTableName, oldTableName))
            {
                var table = new TableContentCheck(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableContentGroup.OldTableNames, oldTableName))
            {
                var table = new TableContentGroup(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableDbCache.OldTableName, oldTableName))
            {
                var table = new TableDbCache(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableErrorLog.OldTableName, oldTableName))
            {
                converter = TableErrorLog.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableLog.OldTableName, oldTableName))
            {
                var table = new TableLog(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TablePermissionsInRoles.OldTableName, oldTableName))
            {
                var table = new TablePermissionsInRoles(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableRelatedField.OldTableNames, oldTableName))
            {
                var table = new TableRelatedField(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableRelatedFieldItem.OldTableNames, oldTableName))
            {
                var table = new TableRelatedFieldItem(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableRole.OldTableName, oldTableName))
            {
                var table = new TableRole(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableSite.OldTableNames, oldTableName))
            {
                var table = new TableSite(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableSiteLog.OldTableNames, oldTableName))
            {
                var table = new TableSiteLog(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableSitePermissions.OldTableNames, oldTableName))
            {
                var table = new TableSitePermissions(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyle.OldTableName, oldTableName))
            {
                var table = new TableTableStyle(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableContentTag.OldTableNames, oldTableName))
            {
                var table = new TableContentTag(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableTemplate.OldTableNames, oldTableName))
            {
                var table = new TableTemplate(_databaseManager);
                converter = table.Converter;
            }
            else if (ListUtils.ContainsIgnoreCase(TableTemplateLog.OldTableNames, oldTableName))
            {
                var table = new TableTemplateLog(_databaseManager);
                converter = table.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableUser.OldTableName, oldTableName))
            {
                var table = new TableUser(_databaseManager);
                converter = table.Converter;
            }

            return await GetNewTableAsync(console, oldTableName, oldTable, converter);
        }
    }
}
