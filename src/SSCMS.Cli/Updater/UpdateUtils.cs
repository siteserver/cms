using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SSCMS.Cli.Core;
using SSCMS.Cli.Updater.Tables;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using TableInfo = SSCMS.Cli.Core.TableInfo;

namespace SSCMS.Cli.Updater
{
    public static class UpdateUtils
    {
        public static string GetConvertValueDictKey(string key, object oldValue)
        {
            return $"{key}${oldValue}";
        }

        public static string GetSplitContentTableName(int siteId)
        {
            return $"siteserver_Content_{siteId}";
        }

        public static List<Dictionary<string, object>> UpdateRows(List<JObject> oldRows, Dictionary<string, string[]> convertKeyDict, Dictionary<string, string> convertValueDict, Func<Dictionary<string, object>, Dictionary<string, object>> process)
        {
            var newRows = new List<Dictionary<string, object>>();

            foreach (var oldRow in oldRows)
            {
                var newRow = TranslateUtils.ToDictionaryIgnoreCase(oldRow);
                foreach (var key in convertKeyDict.Keys)
                {
                    var convertKeys = convertKeyDict[key];
                    foreach (var convertKey in convertKeys)
                    {
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
                    }
                    //var value = newRow [convertKeyDict[key]];

                    //var valueDictKey = GetConvertValueDictKey(key, value);
                    //if (convertValueDict != null && convertValueDict.ContainsKey(valueDictKey))
                    //{
                    //    value = convertValueDict[valueDictKey];
                    //}

                    //newRow[key] = value;
                }

                if (process != null && newRow != null)
                {
                    newRow = process(newRow);
                }

                if (newRow != null)
                {
                    newRows.Add(newRow);
                }
            }

            return newRows;
        }

        public static void LoadSites(ISettingsManager settingsManager, TreeInfo oldTreeInfo, List<int> siteIdList, List<string> tableNames)
        {
            foreach(var oldSiteTableName in TableSite.OldTableNames)
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
                            var dict = TranslateUtils.ToDictionaryIgnoreCase(row);
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
                                if (!string.IsNullOrEmpty(value) && !tableNames.Contains(value))
                                {
                                    tableNames.Add(value);
                                }
                            }
                        }
                    }
                }
            }

            var siteTableName = settingsManager.Database.GetTableName<Site>();
            var metadataFilePath = oldTreeInfo.GetTableMetadataFilePath(siteTableName);
            if (FileUtils.IsFileExists(metadataFilePath))
            {
                var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(metadataFilePath, Encoding.UTF8));
                foreach (var fileName in siteTableInfo.RowFiles)
                {
                    var filePath = oldTreeInfo.GetTableContentFilePath(siteTableName, fileName);
                    var rows = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                    foreach (var row in rows)
                    {
                        var dict = TranslateUtils.ToDictionaryIgnoreCase(row);
                        if (dict.ContainsKey(nameof(Site.Id)))
                        {
                            var value = Convert.ToInt32(dict[nameof(Site.Id)]);
                            if (value > 0 && !siteIdList.Contains(value))
                            {
                                siteIdList.Add(value);
                            }
                        }
                        if (dict.ContainsKey(nameof(Site.TableName)))
                        {
                            var value = Convert.ToString(dict[nameof(Site.TableName)]);
                            if (!string.IsNullOrEmpty(value) && !tableNames.Contains(value))
                            {
                                tableNames.Add(value);
                            }
                        }
                    }
                }
            }
        }

        public static async Task UpdateSitesSplitTableNameAsync(IDatabaseManager databaseManager, TreeInfo newTreeInfo, Dictionary<int, TableInfo> splitSiteTableDict)
        {
            var siteMetadataFilePath = newTreeInfo.GetTableMetadataFilePath(databaseManager.SiteRepository.TableName);
            if (FileUtils.IsFileExists(siteMetadataFilePath))
            {
                var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(siteMetadataFilePath, Encoding.UTF8));
                foreach (var fileName in siteTableInfo.RowFiles)
                {
                    var filePath = newTreeInfo.GetTableContentFilePath(databaseManager.SiteRepository.TableName, fileName);
                    var oldRows = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                    var newRows = new List<Dictionary<string, object>>();
                    foreach (var row in oldRows)
                    {
                        var dict = TranslateUtils.ToDictionaryIgnoreCase(row);
                        if (dict.ContainsKey(nameof(Site.Id)))
                        {
                            var siteId = Convert.ToInt32(dict[nameof(Site.Id)]);
                            dict[nameof(Site.TableName)] = UpdateUtils.GetSplitContentTableName(siteId);
                        }

                        newRows.Add(dict);
                    }

                    await FileUtils.WriteTextAsync(filePath, TranslateUtils.JsonSerialize(newRows));
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
            //    var filePath = newTreeInfo.GetTableContentFilePath(DataProvider.SiteRepository.TableName, siteTableInfo.RowFiles[siteTableInfo.RowFiles.Count]);
            //    var tableInfoList = TranslateUtils.JsonDeserialize<List<CMS.Model.TableInfo>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                


            //    await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(tableInfoList));
            //}
        }
    }
}
