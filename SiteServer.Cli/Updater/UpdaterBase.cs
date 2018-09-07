using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.Utils;

namespace SiteServer.Cli.Updater
{
    public abstract class UpdaterBase
    {
        protected UpdaterBase(TreeInfo oldTreeInfo, TreeInfo newTreeInfo)
        {
            OldTreeInfo = oldTreeInfo;
            NewTreeInfo = newTreeInfo;
        }

        protected TreeInfo OldTreeInfo { get; }

        protected TreeInfo NewTreeInfo { get; }

        public async Task<Tuple<string, TableInfo>> GetNewTableInfoAsync(string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await CliUtils.PrintRowAsync(oldTableName, "Abandon", "--");
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

            await CliUtils.PrintRowAsync(oldTableName, converter.NewTableName, oldTableInfo.TotalCount.ToString("#,0"));

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

                            var newRows = UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict);

                            await FileUtils.WriteTextAsync(newFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
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

        public async Task<List<Tuple<int, string, TableInfo>>> GetNewSplitContentsTableInfoAsync(List<int> siteIdList, string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await CliUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return null;
            }

            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            await CliUtils.PrintRowAsync(oldTableName, "#split-content#", oldTableInfo.TotalCount.ToString("#,0"));

            var newRows = new List<Dictionary<string, object>>();

            foreach (var fileName in oldTableInfo.RowFiles)
            {
                var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);

                var oldRows =
                    TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                newRows.AddRange(UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict));
            }

            var siteIdWithRows = new Dictionary<int, List<Dictionary<string, object>>>();
            foreach (var siteId in siteIdList)
            {
                siteIdWithRows.Add(siteId, new List<Dictionary<string, object>>());
            }

            foreach (var newRow in newRows)
            {
                if (newRow.ContainsKey("siteId"))
                {
                    var siteId = (int)newRow["siteId"];
                    if (siteIdList.Contains(siteId))
                    {
                        var rows = siteIdWithRows[siteId];
                        rows.Add(newRow);
                    }
                }
            }

            var tupleList = new List<Tuple<int, string, TableInfo>>();

            foreach (var siteId in siteIdList)
            {
                var rows = siteIdWithRows[siteId];

                var tableName = UpdateUtils.GetSplitContentTableName(siteId);

                var tableInfo = new TableInfo
                {
                    Columns = converter.NewColumns,
                    TotalCount = rows.Count,
                    RowFiles = new List<string>()
                };

                if (tableInfo.TotalCount > 0)
                {
                    var current = 1;
                    if (tableInfo.TotalCount > CliUtils.PageSize)
                    {
                        var pageCount = (int)Math.Ceiling((double)tableInfo.TotalCount / CliUtils.PageSize);

                        using (var progress = new ProgressBar())
                        {
                            for (; current <= pageCount; current++)
                            {
                                progress.Report((double)(current - 1) / pageCount);

                                var fileName = $"{current}.json";
                                tableInfo.RowFiles.Add(fileName);
                                var offset = (current - 1) * CliUtils.PageSize;
                                var limit = tableInfo.TotalCount - offset < CliUtils.PageSize ? tableInfo.TotalCount - offset : CliUtils.PageSize;

                                var pageRows = rows.Skip(offset).Take(limit);

                                //var rows = DataProvider.DatabaseDao.GetPageObjects(tableName, identityColumnName, offset, limit);

                                var filePath = NewTreeInfo.GetTableContentFilePath(tableName, fileName);

                                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(pageRows));
                            }
                        }
                    }
                    else
                    {
                        var fileName = $"{current}.json";
                        tableInfo.RowFiles.Add(fileName);

                        var filePath = NewTreeInfo.GetTableContentFilePath(tableName, fileName);

                        await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(rows));
                    }
                }

                tupleList.Add(new Tuple<int, string, TableInfo>(siteId, tableName, tableInfo));
            }

            return tupleList;
        }

        public abstract Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob);
    }
}
