using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater.Tables;
using SiteServer.Cli.Updater.Tables.GovInteract;
using SiteServer.Cli.Updater.Tables.GovPublic;
using SiteServer.Cli.Updater.Tables.Jobs;
using SiteServer.CMS.Core;
using SiteServer.CMS.Provider;
using SiteServer.Utils;

namespace SiteServer.Cli.Updater
{
    public class UpdaterManager
    {
        public UpdaterManager(TreeInfo oldTreeInfo, TreeInfo newTreeInfo)
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

        public async Task UpdateSplitContentsTableInfoAsync(Dictionary<int, TableInfo> splitSiteTableDict, List<int> siteIdList, string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await CliUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return;
            }

            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            await CliUtils.PrintRowAsync(oldTableName, "#split-content#", oldTableInfo.TotalCount.ToString("#,0"));

            if (oldTableInfo.RowFiles.Count > 0)
            {
                var i = 0;
                using (var progress = new ProgressBar())
                {
                    foreach (var fileName in oldTableInfo.RowFiles)
                    {
                        progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                        var newRows = new List<Dictionary<string, object>>();

                        var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);

                        var oldRows =
                            TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                        newRows.AddRange(UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict));

                        var siteIdWithRows = new Dictionary<int, List<Dictionary<string, object>>>();
                        foreach (var siteId in siteIdList)
                        {
                            siteIdWithRows.Add(siteId, new List<Dictionary<string, object>>());
                        }

                        foreach (var newRow in newRows)
                        {
                            if (newRow.ContainsKey(nameof(CMS.Model.ContentInfo.SiteId)))
                            {
                                var siteId = Convert.ToInt32(newRow[nameof(CMS.Model.ContentInfo.SiteId)]);
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
                            var siteTableName = ContentDao.GetContentTableName(siteId);
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
                                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(siteRows));
                            }
                        }
                    }
                }
            }
        }

        public async Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob)
        {
            ConvertInfo converter = null;

            if (StringUtils.EqualsIgnoreCase(TableAdministrator.OldTableName, oldTableName))
            {
                converter = TableAdministrator.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableAdministratorsInRoles.OldTableName, oldTableName))
            {
                converter = TableAdministratorsInRoles.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableArea.OldTableName, oldTableName))
            {
                converter = TableArea.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableChannel.OldTableNames, oldTableName))
            {
                converter = TableChannel.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableChannelGroup.OldTableNames, oldTableName))
            {
                converter = TableChannelGroup.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableConfig.OldTableName, oldTableName))
            {
                converter = TableConfig.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableContentCheck.OldTableName, oldTableName))
            {
                converter = TableContentCheck.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableContentGroup.OldTableNames, oldTableName))
            {
                converter = TableContentGroup.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableDbCache.OldTableName, oldTableName))
            {
                converter = TableDbCache.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableDepartment.OldTableName, oldTableName))
            {
                converter = TableDepartment.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableErrorLog.OldTableName, oldTableName))
            {
                converter = TableErrorLog.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableKeyword.OldTableNames, oldTableName))
            {
                converter = TableKeyword.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableLog.OldTableName, oldTableName))
            {
                converter = TableLog.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TablePermissionsInRoles.OldTableName, oldTableName))
            {
                converter = TablePermissionsInRoles.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableRelatedField.OldTableNames, oldTableName))
            {
                converter = TableRelatedField.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableRelatedFieldItem.OldTableNames, oldTableName))
            {
                converter = TableRelatedFieldItem.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableRole.OldTableName, oldTableName))
            {
                converter = TableRole.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableSite.OldTableNames, oldTableName))
            {
                converter = TableSite.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableSiteLog.OldTableNames, oldTableName))
            {
                converter = TableSiteLog.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableSitePermissions.OldTableNames, oldTableName))
            {
                converter = TableSitePermissions.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyle.OldTableName, oldTableName))
            {
                converter = TableTableStyle.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyleItem.OldTableName, oldTableName))
            {
                converter = TableTableStyleItem.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTag.OldTableName, oldTableName))
            {
                converter = TableTag.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplate.OldTableNames, oldTableName))
            {
                converter = TableTemplate.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplateLog.OldTableNames, oldTableName))
            {
                converter = TableTemplateLog.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplateMatch.OldTableNames, oldTableName))
            {
                converter = TableTemplateMatch.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableUser.OldTableName, oldTableName))
            {
                converter = TableUser.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableUserLog.OldTableName, oldTableName))
            {
                converter = TableUserLog.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractChannel.OldTableName, oldTableName))
            {
                converter = TableGovInteractChannel.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractLog.OldTableName, oldTableName))
            {
                converter = TableGovInteractLog.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractPermissions.OldTableName, oldTableName))
            {
                converter = TableGovInteractPermissions.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractRemark.OldTableName, oldTableName))
            {
                converter = TableGovInteractRemark.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractReply.OldTableName, oldTableName))
            {
                converter = TableGovInteractReply.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractType.OldTableName, oldTableName))
            {
                converter = TableGovInteractType.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicCategory.OldTableName, oldTableName))
            {
                converter = TableGovPublicCategory.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicCategoryClass.OldTableName, oldTableName))
            {
                converter = TableGovPublicCategoryClass.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicIdentifierRule.OldTableName, oldTableName))
            {
                converter = TableGovPublicIdentifierRule.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicIdentifierSeq.OldTableName, oldTableName))
            {
                converter = TableGovPublicIdentifierSeq.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(tableNameListForGovPublic, oldTableName))
            {
                converter = TableGovPublicContent.GetConverter(oldTableInfo.Columns);
            }
            else if (StringUtils.ContainsIgnoreCase(tableNameListForGovInteract, oldTableName))
            {
                converter = TableGovInteractContent.GetConverter(oldTableInfo.Columns);
            }
            else if (StringUtils.ContainsIgnoreCase(tableNameListForJob, oldTableName))
            {
                converter = TableJobsContent.GetConverter(oldTableInfo.Columns);
            }

            return await GetNewTableInfoAsync(oldTableName, oldTableInfo, converter);
        }
    }
}
