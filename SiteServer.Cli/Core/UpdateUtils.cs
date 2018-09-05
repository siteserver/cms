using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
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

        public static void LoadContentTableNameList(TreeInfo oldTreeInfo, string oldSiteTableName, List<string> tableNameListForContent, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob)
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
    }
}
