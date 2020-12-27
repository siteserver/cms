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
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using TableInfo = SSCMS.Cli.Core.TableInfo;

namespace SSCMS.Cli.Services
{
    public class DataUpdateService : IDataUpdateService
    {
        private readonly IDatabaseManager _databaseManager;

        public DataUpdateService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        protected TreeInfo OldTreeInfo { get; private set; }

        protected TreeInfo NewTreeInfo { get; private set; }

        public void Load(TreeInfo oldTreeInfo, TreeInfo newTreeInfo)
        {
            OldTreeInfo = oldTreeInfo;
            NewTreeInfo = newTreeInfo;
        }

        public async Task<Tuple<string, TableInfo>> GetNewTableInfoAsync(string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await WriteUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return null;
            }

            if (string.IsNullOrEmpty(converter.NewTableName))
            {
                converter.NewTableName = oldTableName;
            }
            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            var newTableInfo = new TableInfo
            {
                Columns = converter.NewColumns,
                TotalCount = oldTableInfo.TotalCount,
                RowFiles = oldTableInfo.RowFiles
            };

            await WriteUtils.PrintRowAsync(oldTableName, converter.NewTableName, oldTableInfo.TotalCount.ToString("#,0"));

            if (oldTableInfo.RowFiles.Count > 0)
            {
                var i = 0;
                using (var progress = new ProgressBar())
                {
                    foreach (var fileName in oldTableInfo.RowFiles)
                    {
                        progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                        var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);
                        var newFilePath = NewTreeInfo.GetTableContentFilePath(converter.NewTableName, fileName);

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
            }

            return new Tuple<string, TableInfo>(converter.NewTableName, newTableInfo);
        }

        public async Task UpdateSplitContentsTableInfoAsync(Dictionary<int, TableInfo> splitSiteTableDict, List<int> siteIdList, string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await WriteUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return;
            }

            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            await WriteUtils.PrintRowAsync(oldTableName, "#split-contents#", oldTableInfo.TotalCount.ToString("#,0"));

            if (oldTableInfo.RowFiles.Count > 0)
            {
                var i = 0;
                using var progress = new ProgressBar();
                foreach (var fileName in oldTableInfo.RowFiles)
                {
                    progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                    var newRows = new List<Dictionary<string, object>>();

                    var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);

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

                        var siteTableInfo = splitSiteTableDict[siteId];
                        siteTableInfo.TotalCount += siteRows.Count;

                        foreach(var tableColumn in converter.NewColumns)
                        {
                            if (!siteTableInfo.Columns.Any(t => StringUtils.EqualsIgnoreCase(t.AttributeName, tableColumn.AttributeName)))
                            {
                                siteTableInfo.Columns.Add(tableColumn);
                            }
                        }

                        if (siteRows.Count > 0)
                        {
                            var siteTableFileName = $"{siteTableInfo.RowFiles.Count + 1}.json";
                            siteTableInfo.RowFiles.Add(siteTableFileName);
                            var filePath = NewTreeInfo.GetTableContentFilePath(siteTableName, siteTableFileName);
                            await FileUtils.WriteTextAsync(filePath, TranslateUtils.JsonSerialize(siteRows));
                        }
                    }
                }
            }
        }

        public async Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo)
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

            return await GetNewTableInfoAsync(oldTableName, oldTableInfo, converter);
        }
    }
}
