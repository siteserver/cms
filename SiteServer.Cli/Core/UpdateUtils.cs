using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Updater.Plugins.GovInteract;
using SiteServer.Cli.Updater.Plugins.GovPublic;
using SiteServer.Cli.Updater.Plugins.Jobs;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
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

        public static void LoadContentTableNameList(TreeInfo oldTreeInfo, string oldSiteTableName, List<string> contentTableNameList)
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
                        if (dict.TryGetValue("AuxiliaryTableForContent",
                            out obj))
                        {
                            if (obj != null && !contentTableNameList.Contains(obj.ToString()))
                            {
                                contentTableNameList.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForGovInteract",
                            out obj))
                        {
                            if (obj != null && !contentTableNameList.Contains(obj.ToString()))
                            {
                                contentTableNameList.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForGovPublic",
                            out obj))
                        {
                            if (obj != null && !contentTableNameList.Contains(obj.ToString()))
                            {
                                contentTableNameList.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForJob",
                            out obj))
                        {
                            if (obj != null && !contentTableNameList.Contains(obj.ToString()))
                            {
                                contentTableNameList.Add(obj.ToString());
                            }
                        }
                        if (dict.TryGetValue("AuxiliaryTableForVote",
                            out obj))
                        {
                            if (obj != null && !contentTableNameList.Contains(obj.ToString()))
                            {
                                contentTableNameList.Add(obj.ToString());
                            }
                        }
                    }
                }
            }
        }

        public static string GetContentTableName(string oldTableNameWithPrefix)
        {
            var newTableName = oldTableNameWithPrefix;

            if (StringUtils.EqualsIgnoreCase(oldTableNameWithPrefix, TableGovInteractContent.OldTableName))
            {
                newTableName = TableGovInteractContent.NewTableName;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithPrefix, TableGovPublicContent.OldTableName))
            {
                newTableName = TableGovPublicContent.NewTableName;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithPrefix, TableJobsContent.OldTableName))
            {
                newTableName = TableJobsContent.NewTableName;
            }

            return newTableName;
        }
    }
}
