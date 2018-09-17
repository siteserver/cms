using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class TableColumnManager
    {
        private static class TableColumnManagerCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TableColumnManager));
            //private static readonly FileWatcherClass FileWatcher;

            //static TableColumnManagerCache()
            //{
            //    FileWatcher = new FileWatcherClass(FileWatcherClass.TableColumn);
            //    FileWatcher.OnFileChange += FileWatcher_OnFileChange;
            //}

            //private static void FileWatcher_OnFileChange(object sender, EventArgs e)
            //{
            //    CacheManager.Remove(CacheKey);
            //}

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
                //FileWatcher.UpdateCacheFile();
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
                var allDict = DataCacheManager.Get<Dictionary<string, List<TableColumn>>>(CacheKey);
                if (allDict != null) return allDict;

                allDict = new Dictionary<string, List<TableColumn>>();
                DataCacheManager.InsertHours(CacheKey, allDict, 24);
                return allDict;
            }

            public static List<TableColumn> GetTableColumnInfoList(string tableName)
            {
                var allDict = GetAllDictionary();

                List<TableColumn> list;
                allDict.TryGetValue(tableName, out list);

                if (list != null) return list;

                list = DataProvider.DatabaseDao.GetTableColumnInfoList(WebConfigUtils.ConnectionString, tableName);
                Update(allDict, list, tableName);
                return list;
            }
        }

        public static List<TableColumn> GetTableColumnInfoList(string tableName)
        {
            return TableColumnManagerCache.GetTableColumnInfoList(tableName);
        }

        public static List<TableColumn> GetTableColumnInfoList(string tableName, List<string> excludeAttributeNameList)
        {
            var list = TableColumnManagerCache.GetTableColumnInfoList(tableName);
            if (excludeAttributeNameList == null || excludeAttributeNameList.Count == 0) return list;

            return list.Where(tableColumnInfo =>
                !StringUtils.ContainsIgnoreCase(excludeAttributeNameList, tableColumnInfo.AttributeName)).ToList();
        }

        public static List<TableColumn> GetTableColumnInfoList(string tableName, DataType excludeDataType)
        {
            var list = TableColumnManagerCache.GetTableColumnInfoList(tableName);

            return list.Where(tableColumnInfo =>
                tableColumnInfo.DataType != excludeDataType).ToList();
        }

        public static TableColumn GetTableColumnInfo(string tableName, string attributeName)
        {
            var list = TableColumnManagerCache.GetTableColumnInfoList(tableName);
            return list.FirstOrDefault(tableColumnInfo =>
                StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, attributeName));
        }

        public static bool IsAttributeNameExists(string tableName, string attributeName)
        {
            var list = TableColumnManagerCache.GetTableColumnInfoList(tableName);
            return list.Any(tableColumnInfo =>
                StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, attributeName));
        }

        public static List<string> GetTableColumnNameList(string tableName)
        {
            var allTableColumnInfoList = GetTableColumnInfoList(tableName);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public static List<string> GetTableColumnNameList(string tableName, List<string> excludeAttributeNameList)
        {
            var allTableColumnInfoList = GetTableColumnInfoList(tableName, excludeAttributeNameList);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public static List<string> GetTableColumnNameList(string tableName, DataType excludeDataType)
        {
            var allTableColumnInfoList = GetTableColumnInfoList(tableName, excludeDataType);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public static void ClearCache()
        {
            TableColumnManagerCache.Clear();
        }
    }

}
