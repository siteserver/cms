using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Settings;
using SS.CMS.Data;
using SS.CMS.Utils;
using AppContext = SS.CMS.Core.Settings.AppContext;

namespace SS.CMS.Core.Cache
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

                list = AppContext.Db.GetTableColumns(tableName);
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

        public static bool CreateTable(string tableName, List<TableColumn> tableColumns, string pluginId, bool isContentTable, out Exception ex)
        {
            ex = null;

            try
            {
                AppContext.Db.CreateTable(tableName, tableColumns);
            }
            catch (Exception e)
            {
                ex = e;
                LogUtils.AddErrorLog(pluginId, ex, string.Empty);
                return false;
            }

            if (isContentTable)
            {
                try
                {
                    AppContext.Db.CreateIndex(tableName, $"IX_{tableName}_General", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception e)
                {
                    ex = e;
                    LogUtils.AddErrorLog(pluginId, ex, string.Empty);
                    return false;
                }

                try
                {
                    AppContext.Db.CreateIndex(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_Taxis")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception e)
                {
                    ex = e;
                    LogUtils.AddErrorLog(pluginId, ex, string.Empty);
                    return false;
                }
            }

            ClearCache();
            return true;
        }

        public static void AlterTable(string tableName, List<TableColumn> tableColumns, string pluginId, List<string> dropColumnNames = null)
        {
            try
            {
                AppContext.Db.AlterTable(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);

                ClearCache();
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex, string.Empty);
            }
        }

        private static IList<TableColumn> GetRealTableColumns(IEnumerable<TableColumn> tableColumns)
        {
            var realTableColumns = new List<TableColumn>();
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Guid)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.LastModifiedDate)))
                {
                    continue;
                }

                if (tableColumn.DataType == DataType.VarChar && tableColumn.DataLength == 0)
                {
                    tableColumn.DataLength = 2000;
                }
                realTableColumns.Add(tableColumn);
            }

            realTableColumns.InsertRange(0, new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = nameof(Entity.Id),
                    DataType = DataType.Integer,
                    IsIdentity = true,
                    IsPrimaryKey = true
                },
                new TableColumn
                {
                    AttributeName = nameof(Entity.Guid),
                    DataType = DataType.VarChar,
                    DataLength = 50
                },
                new TableColumn
                {
                    AttributeName = nameof(Entity.LastModifiedDate),
                    DataType = DataType.DateTime
                }
            });

            return realTableColumns;
        }

        public static void CreateContentTable(string tableName, List<TableColumn> tableColumns)
        {
            var isDbExists = DatabaseUtils.IsTableExists(tableName);
            if (isDbExists) return;

            AppContext.Db.CreateTable(tableName, tableColumns);
            AppContext.Db.CreateIndex(tableName, $"IX_{tableName}", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");
            AppContext.Db.CreateIndex(tableName, $"IX_{tableName}_Taxis", ContentAttribute.Taxis);

            ClearCache();
        }

        public static void AlterSystemTable(string tableName, List<TableColumn> tableColumns, List<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = GetTableColumnNameList(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (StringUtils.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    var databaseColumn = GetTableColumnInfo(tableName, tableColumn.AttributeName);
                    if (databaseColumn != null && !tableColumn.IsIdentity)
                    {
                        if (tableColumn.DataType != databaseColumn.DataType ||
                            tableColumn.DataType == databaseColumn.DataType && tableColumn.DataLength > databaseColumn.DataLength)
                        {
                            list.Add(SqlUtils.GetModifyColumnsSqlString(tableName, tableColumn.AttributeName, SqlUtils.GetColumnTypeString(tableColumn)));
                        }
                    }
                }
                else
                {
                    list.Add(SqlUtils.GetAddColumnsSqlString(tableName, SqlUtils.GetColumnSqlString(tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (StringUtils.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(SqlUtils.GetDropColumnsSqlString(tableName, columnName));
                    }
                }
            }

            if (list.Count > 0)
            {
                foreach (var sqlString in list)
                {
                    try
                    {
                        DatabaseUtils.Execute(sqlString);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(ex, sqlString);
                    }
                }

                ClearCache();
            }
        }
    }

}
