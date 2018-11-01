using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater.Tables;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Provider;
using SiteServer.Utils;
using TableInfo = SiteServer.Cli.Core.TableInfo;

namespace SiteServer.Cli.Updater
{
    public static class UpdateUtils
    {
        public static string GetConvertValueDictKey(string key, object oldValue)
        {
            return $"{key}${oldValue}";
        }

        public static List<Dictionary<string, object>> UpdateRows(List<JObject> oldRows, Dictionary<string, string> convertKeyDict, Dictionary<string, string> convertValueDict)
        {
            var newRows = new List<Dictionary<string, object>>();

            foreach (var oldRow in oldRows)
            {
                var newRow = TranslateUtils.JsonGetDictionaryIgnorecase(oldRow);
                foreach (var key in convertKeyDict.Keys)
                {
                    var convertKey = convertKeyDict[key];
                    object value;
                    if (newRow.TryGetValue(convertKey, out value))
                    {
                        var valueDictKey = GetConvertValueDictKey(key, value);
                        if (convertValueDict != null && convertValueDict.ContainsKey(valueDictKey))
                        {
                            value = convertValueDict[valueDictKey];
                        }

                        newRow[key] = value;
                    }
                    //var value = newRow [convertKeyDict[key]];

                    //var valueDictKey = GetConvertValueDictKey(key, value);
                    //if (convertValueDict != null && convertValueDict.ContainsKey(valueDictKey))
                    //{
                    //    value = convertValueDict[valueDictKey];
                    //}

                    //newRow[key] = value;
                }

                newRows.Add(newRow);
            }

            return newRows;
        }

        public static void LoadSites(TreeInfo oldTreeInfo, List<int> siteIdList, List<string> tableNameListForContent, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob)
        {
            foreach(string oldSiteTableName in TableSite.OldTableNames)
            {
                var siteMetadataFilePath = oldTreeInfo.GetTableMetadataFilePath(oldSiteTableName);
                if (FileUtils.IsFileExists(siteMetadataFilePath))
                {
                    var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(siteMetadataFilePath, Encoding.UTF8));
                    foreach (var fileName in siteTableInfo.RowFiles)
                    {
                        var filePath = oldTreeInfo.GetTableContentFilePath(oldSiteTableName, fileName);
                        var rows = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                        foreach (var row in rows)
                        {
                            var dict = TranslateUtils.JsonGetDictionaryIgnorecase(row);
                            if (dict.ContainsKey(nameof(TableSite.PublishmentSystemId)))
                            {
                                var value = Convert.ToInt32(dict[nameof(TableSite.PublishmentSystemId)]);
                                if (value > 0 && !siteIdList.Contains(value))
                                {
                                    siteIdList.Add(value);
                                }
                            }
                            if (dict.ContainsKey(nameof(TableSite.AuxiliaryTableForContent)))
                            {
                                var value = Convert.ToString(dict[nameof(TableSite.AuxiliaryTableForContent)]);
                                if (!string.IsNullOrEmpty(value) && !tableNameListForContent.Contains(value))
                                {
                                    tableNameListForContent.Add(value);
                                }
                            }
                            if (dict.ContainsKey(nameof(TableSite.AuxiliaryTableForGovInteract)))
                            {
                                var value = Convert.ToString(dict[nameof(TableSite.AuxiliaryTableForGovInteract)]);
                                if (!string.IsNullOrEmpty(value) && !tableNameListForGovInteract.Contains(value))
                                {
                                    tableNameListForGovInteract.Add(value);
                                }
                            }
                            if (dict.ContainsKey(nameof(TableSite.AuxiliaryTableForGovPublic)))
                            {
                                var value = Convert.ToString(dict[nameof(TableSite.AuxiliaryTableForGovPublic)]);
                                if (!string.IsNullOrEmpty(value) && !tableNameListForGovPublic.Contains(value))
                                {
                                    tableNameListForGovPublic.Add(value);
                                }
                            }
                            if (dict.ContainsKey(nameof(TableSite.AuxiliaryTableForJob)))
                            {
                                var value = Convert.ToString(dict[nameof(TableSite.AuxiliaryTableForJob)]);
                                if (!string.IsNullOrEmpty(value) && !tableNameListForJob.Contains(value))
                                {
                                    tableNameListForJob.Add(value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task UpdateSitesSplitTableNameAsync(TreeInfo newTreeInfo, Dictionary<int, TableInfo> splitSiteTableDict)
        {
            var siteMetadataFilePath = newTreeInfo.GetTableMetadataFilePath(DataProvider.SiteDao.TableName);
            if (FileUtils.IsFileExists(siteMetadataFilePath))
            {
                var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(siteMetadataFilePath, Encoding.UTF8));
                foreach (var fileName in siteTableInfo.RowFiles)
                {
                    var filePath = newTreeInfo.GetTableContentFilePath(DataProvider.SiteDao.TableName, fileName);
                    var oldRows = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                    var newRows = new List<Dictionary<string, object>>();
                    foreach (var row in oldRows)
                    {
                        var dict = TranslateUtils.JsonGetDictionaryIgnorecase(row);
                        if (dict.ContainsKey(nameof(SiteInfo.Id)))
                        {
                            var siteId = Convert.ToInt32(dict[nameof(SiteInfo.Id)]);
                            dict[nameof(SiteInfo.TableName)] = ContentDao.GetContentTableName(siteId);
                        }

                        newRows.Add(dict);
                    }

                    await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                }
            }

            //foreach (var siteId in splitSiteTableDict.Keys)
            //{
            //    var siteTableInfo = splitSiteTableDict[siteId];
            //    var siteTableName = UpdateUtils.GetSplitContentTableName(siteId);
                
            //    siteTableInfo.Columns
            //}

            //var tableFilePath = newTreeInfo.GetTableMetadataFilePath(DataProvider.TableDao.TableName);
            //if (FileUtils.IsFileExists(tableFilePath))
            //{
            //    var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(tableFilePath, Encoding.UTF8));
            //    var filePath = newTreeInfo.GetTableContentFilePath(DataProvider.SiteDao.TableName, siteTableInfo.RowFiles[siteTableInfo.RowFiles.Count]);
            //    var tableInfoList = TranslateUtils.JsonDeserialize<List<CMS.Model.TableInfo>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                


            //    await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(tableInfoList));
            //}
        }
    }
}
