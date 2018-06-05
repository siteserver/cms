using System;
using System.Collections.Generic;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.IO;

namespace SiteServer.CMS.Core
{
    public static class TableColumnManager
    {
        private static class TableColumnManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "SiteServer.CMS.Core.TableColumnManager";
            private static readonly FileWatcherClass FileWatcher;

            static TableColumnManagerCache()
            {
                FileWatcher = new FileWatcherClass(FileWatcherClass.TableColumn);
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

            private static void Update(Dictionary<string, List<TableColumn>> allDict, List<TableColumn> list,
                string key)
            {
                lock (LockObject)
                {
                    allDict[key] = list;
                }
            }

            private static Dictionary<string, List<TableColumn>> GetAllDictionary()
            {
                var allDict = CacheUtils.Get(CacheKey) as Dictionary<string, List<TableColumn>>;
                if (allDict != null) return allDict;

                allDict = new Dictionary<string, List<TableColumn>>();
                CacheUtils.InsertHours(CacheKey, allDict, 24);
                return allDict;
            }

            public static List<TableColumn> GetTableColumnInfoListLowercase(string tableName)
            {
                var allDict = GetAllDictionary();

                List<TableColumn> list;
                allDict.TryGetValue(tableName, out list);

                if (list != null) return list;

                list = DataProvider.DatabaseDao.GetTableColumnInfoListLowercase(WebConfigUtils.ConnectionString, tableName);
                Update(allDict, list, tableName);
                return list;
            }
        }

        public static List<TableColumn> GetTableColumnInfoListLowercase(string tableName, List<string> excludeAttributeNameListLowercase = null)
        {
            var list = TableColumnManagerCache.GetTableColumnInfoListLowercase(tableName);
            if (excludeAttributeNameListLowercase == null || excludeAttributeNameListLowercase.Count == 0) return list;

            var retval = new List<TableColumn>();
            foreach (var tableColumnInfo in list)
            {
                if (!excludeAttributeNameListLowercase.Contains(tableColumnInfo.AttributeName.ToLower()))
                {
                    retval.Add(tableColumnInfo);
                }
            }

            return retval;
        }

        public static List<string> GetTableColumnNameListLowercase(string tableName, List<string> excludeAttributeNameListLowercase = null)
        {
            var allTableColumnInfoList = GetTableColumnInfoListLowercase(tableName, excludeAttributeNameListLowercase);

            var columnNameList = new List<string>();

            foreach (var tableColumnInfo in allTableColumnInfoList)
            {
                columnNameList.Add(tableColumnInfo.AttributeName);
            }

            return columnNameList;
        }

        public static void ClearCache()
        {
            TableColumnManagerCache.Clear();
        }
    }

}
