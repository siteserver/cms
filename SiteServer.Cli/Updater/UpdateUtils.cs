using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.CMS.Model;
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

        public static void LoadSites(TreeInfo oldTreeInfo, string oldSiteTableName, List<int> siteIdList, List<string> tableNameListForContent, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob)
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
                        object obj;
                        if (dict.TryGetValue("PublishmentSystemID",
                            out obj))
                        {
                            if (obj != null && !siteIdList.Contains((int)obj))
                            {
                                siteIdList.Add((int)obj);
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForContent",
                            out obj))
                        {
                            if (obj != null && !tableNameListForContent.Contains(obj.ToString()))
                            {
                                tableNameListForContent.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForGovInteract",
                            out obj))
                        {
                            if (obj != null && !tableNameListForGovInteract.Contains(obj.ToString()))
                            {
                                tableNameListForGovInteract.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForGovPublic",
                            out obj))
                        {
                            if (obj != null && !tableNameListForGovPublic.Contains(obj.ToString()))
                            {
                                tableNameListForGovPublic.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForJob",
                            out obj))
                        {
                            if (obj != null && !tableNameListForJob.Contains(obj.ToString()))
                            {
                                tableNameListForJob.Add(obj.ToString());
                            }
                        }
                        //if (dict.TryGetValue("AuxiliaryTableForVote",
                        //    out obj))
                        //{
                        //    if (obj != null && !contentTableNameList.Contains(obj.ToString()))
                        //    {
                        //        contentTableNameList.Add(obj.ToString());
                        //    }
                        //}
                    }
                }
            }
        }

        public static string GetSplitContentTableName(int siteId)
        {
            return $"model_SiteContent_{siteId}";
        }

        public static async Task UpdateSitesSplitTableNameAsync(TreeInfo newTreeInfo, string newSiteTableName)
        {
            var siteMetadataFilePath = newTreeInfo.GetTableMetadataFilePath(newSiteTableName);
            if (FileUtils.IsFileExists(siteMetadataFilePath))
            {
                var siteTableInfo = TranslateUtils.JsonDeserialize<TableInfo>(FileUtils.ReadText(siteMetadataFilePath, Encoding.UTF8));
                foreach (var fileName in siteTableInfo.RowFiles)
                {
                    var filePath = newTreeInfo.GetTableContentFilePath(newSiteTableName, fileName);
                    var oldRows = TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(filePath, Encoding.UTF8));
                    var newRows = new List<Dictionary<string, object>>();
                    foreach (var row in oldRows)
                    {
                        var dict = TranslateUtils.JsonGetDictionaryIgnorecase(row);
                        object obj;
                        if (dict.TryGetValue("Id",
                            out obj))
                        {
                            if (obj != null)
                            {
                                var siteId = (int) obj;
                                dict[nameof(SiteInfo.TableName)] = GetSplitContentTableName(siteId);
                            }
                        }

                        newRows.Add(dict);
                    }

                    await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                }
            }
        }
    }
}
