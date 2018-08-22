using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.IO;

namespace SiteServer.CMS.Core
{
    public static class TableMetadataManager
    {
        private static class TableMetadataManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "BaiRong.Core.Table.TableMetadataManager";
            private static readonly FileWatcherClass FileWatcher;

            static TableMetadataManagerCache()
            {
                FileWatcher = new FileWatcherClass(FileWatcherClass.TableMetadata);
                FileWatcher.OnFileChange += FileWatcher_OnFileChange;
            }

            private static void FileWatcher_OnFileChange(object sender, EventArgs e)
            {
                CacheUtils.Remove(CacheKey);
            }

            public static void Clear()
            {
                CacheUtils.Remove(CacheKey);
                FileWatcher.UpdateCacheFile();
            }

            public static Dictionary<string, List<TableMetadataInfo>> GetTableNameWithTableMetadataInfoDictionary()
            {
                var retval = CacheUtils.Get<Dictionary<string, List<TableMetadataInfo>>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<Dictionary<string, List<TableMetadataInfo>>>(CacheKey);
                    if (retval == null)
                    {
                        retval = DataProvider.TableMetadataDao.GetTableNameWithTableMetadataInfoList();
                        CacheUtils.InsertHours(CacheKey, retval, 24);
                    }
                }

                return retval;
            }
        }

        public static void ClearCache()
        {
            TableMetadataManagerCache.Clear();
        }

        public static List<string> GetAllLowerAttributeNameList(string tableName)
        {
            var list = new List<string>();
            list.AddRange(ContentAttribute.AllAttributesLowercase);
            list.AddRange(GetAttributeNameList(tableName, true));
            return list;
        }

        public static List<string> GetAllLowerAttributeNameListExcludeText(string tableName)
        {
            var list = new List<string>();
            list.AddRange(ContentAttribute.AllAttributesLowercase);
            var metadataInfoList = GetTableMetadataInfoList(tableName);
            foreach (var metadataInfo in metadataInfoList)
            {
                if (metadataInfo.DataType != DataType.Text)
                {
                    list.Add(metadataInfo.AttributeName.ToLower());
                }
            }
            return list;
        }

        /// <summary>
        /// 得到辅助表tableName数据库中的字段名称的集合
        /// </summary>
        public static List<string> GetAttributeNameList(string tableName, bool toLower)
        {
            var tableMetadataInfoList = GetTableMetadataInfoList(tableName);
            return tableMetadataInfoList.Select(tableMetadataInfo => toLower ? tableMetadataInfo.AttributeName.ToLower() : tableMetadataInfo.AttributeName).ToList();
        }

        /// <summary>
        /// 得到辅助表的所有字段名称的集合
        /// </summary>
        public static List<string> GetAttributeNameList(string tableName)
        {
            return GetAttributeNameList(tableName, false);
        }

        public static bool IsAttributeNameExists(string tableName, string attributeName)
        {
            var list = GetAttributeNameList(tableName, true);
            return list.Contains(attributeName.ToLower());
        }

        public static List<TableMetadataInfo> GetTableMetadataInfoList(string tableName)
        {
            var dict = TableMetadataManagerCache.GetTableNameWithTableMetadataInfoDictionary();
            List<TableMetadataInfo> additionMetadataList;
            if (!dict.TryGetValue(tableName, out additionMetadataList)) return new List<TableMetadataInfo>();

            if (additionMetadataList == null || additionMetadataList.Count <= 0) return new List<TableMetadataInfo>();

            var metadataList = new List<TableMetadataInfo>();
            var attributeNames = new List<string>();
            foreach (var metadataInfo in additionMetadataList)
            {
                if (attributeNames.Contains(metadataInfo.AttributeName.ToLower()) ||
                    ContentAttribute.AllAttributesLowercase.Contains(metadataInfo.AttributeName.ToLower()))
                    continue;

                attributeNames.Add(metadataInfo.AttributeName.ToLower());
                metadataList.Add(metadataInfo);
            }

            return metadataList;
        }

        public static TableMetadataInfo GetTableMetadataInfo(string tableName, string attributeName)
        {
            var list = GetTableMetadataInfoList(tableName);
            foreach (var tableMetadataInfo in list)
            {
                if (StringUtils.EqualsIgnoreCase(tableMetadataInfo.AttributeName, attributeName))
                {
                    return tableMetadataInfo;
                }
            }
            return null;
        }

        public static string GetTableMetadataDataType(string tableName, string attributeName)
        {
            var metadataInfo = GetTableMetadataInfo(tableName, attributeName);
            if (metadataInfo != null)
            {
                return DataTypeUtils.GetText(metadataInfo.DataType);
            }
            return string.Empty;
        }

        public static int GetTableMetadataId(string tableName, string attributeName)
        {
            var metadataId = 0;
            var list = GetTableMetadataInfoList(tableName);
            foreach (var tableMetadataInfo in list)
            {
                if (StringUtils.EqualsIgnoreCase(tableMetadataInfo.AttributeName, attributeName))
                {
                    metadataId = tableMetadataInfo.Id;
                    break;
                }
            }
            return metadataId;
        }

        public static string GetSerializedString(string tableName)
        {
            var builder = new StringBuilder();
            var list = GetTableMetadataInfoList(tableName);
            var sortedlist = new SortedList();
            foreach (var metadataInfo in list)
            {
                if (metadataInfo.IsSystem == false)
                {
                    /*
                     * AttributeName,
                     * DataType,
                     * DataLength,
                     * CanBeNull,
                     * DBDefaultValue
                     * */
                    string serialize =
                        $"AttributeName:{metadataInfo.AttributeName};DataType:{metadataInfo.DataType.Value};DataLength={metadataInfo.DataLength}";
                    sortedlist.Add(metadataInfo.AttributeName, serialize);
                }
            }

            foreach (string attributeName in sortedlist.Keys)
            {
                builder.Append(sortedlist[attributeName]);
            }

            return builder.ToString();
        }

        public static string GetTableNameOfArchive(string tableName)
        {
            return tableName + "_Archive";
        }
    }

}
