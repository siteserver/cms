using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common
{
    public class TableManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IDb _db;

        public TableManager(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _db = new Db(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);
        }

        private readonly object LockObject = new object();
        private readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TableManager));

        private void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
            //FileWatcher.UpdateCacheFile();
        }

        private void CacheUpdate(Dictionary<string, List<TableColumn>> allDict, List<TableColumn> list,
            string key)
        {
            lock (LockObject)
            {
                allDict[key] = list;
            }
        }

        private Dictionary<string, List<TableColumn>> CacheGetAllDictionary()
        {
            var allDict = DataCacheManager.Get<Dictionary<string, List<TableColumn>>>(CacheKey);
            if (allDict != null) return allDict;

            allDict = new Dictionary<string, List<TableColumn>>();
            DataCacheManager.InsertHours(CacheKey, allDict, 24);
            return allDict;
        }

        private List<TableColumn> CacheGetTableColumnInfoList(string tableName)
        {
            var allDict = CacheGetAllDictionary();

            List<TableColumn> list;
            allDict.TryGetValue(tableName, out list);

            if (list != null) return list;

            list = _db.GetTableColumns(tableName);
            CacheUpdate(allDict, list, tableName);
            return list;
        }

        public List<TableColumn> GetTableColumnInfoList(string tableName)
        {
            return CacheGetTableColumnInfoList(tableName);
        }

        public List<TableColumn> GetTableColumnInfoList(string tableName, List<string> excludeAttributeNameList)
        {
            var list = CacheGetTableColumnInfoList(tableName);
            if (excludeAttributeNameList == null || excludeAttributeNameList.Count == 0) return list;

            return list.Where(tableColumnInfo =>
                !StringUtils.ContainsIgnoreCase(excludeAttributeNameList, tableColumnInfo.AttributeName)).ToList();
        }

        public List<TableColumn> GetTableColumnInfoList(string tableName, DataType excludeDataType)
        {
            var list = CacheGetTableColumnInfoList(tableName);

            return list.Where(tableColumnInfo =>
                tableColumnInfo.DataType != excludeDataType).ToList();
        }

        public TableColumn GetTableColumnInfo(string tableName, string attributeName)
        {
            var list = CacheGetTableColumnInfoList(tableName);
            return list.FirstOrDefault(tableColumnInfo =>
                StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, attributeName));
        }

        public bool IsAttributeNameExists(string tableName, string attributeName)
        {
            var list = CacheGetTableColumnInfoList(tableName);
            return list.Any(tableColumnInfo =>
                StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, attributeName));
        }

        public List<string> GetTableColumnNameList(string tableName)
        {
            var allTableColumnInfoList = GetTableColumnInfoList(tableName);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public List<string> GetTableColumnNameList(string tableName, List<string> excludeAttributeNameList)
        {
            var allTableColumnInfoList = GetTableColumnInfoList(tableName, excludeAttributeNameList);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public List<string> GetTableColumnNameList(string tableName, DataType excludeDataType)
        {
            var allTableColumnInfoList = GetTableColumnInfoList(tableName, excludeDataType);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public bool CreateTable(string tableName, List<TableColumn> tableColumns, string pluginId, bool isContentTable, out Exception ex)
        {
            ex = null;

            try
            {
                _db.CreateTable(tableName, tableColumns);
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
                    _db.CreateIndex(tableName, $"IX_{tableName}_General", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");


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
                    _db.CreateIndex(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

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

        public void AlterTable(string tableName, List<TableColumn> tableColumns, string pluginId, List<string> dropColumnNames = null)
        {
            try
            {
                _db.AlterTable(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);

                ClearCache();
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(pluginId, ex, string.Empty);
            }
        }

        private IList<TableColumn> GetRealTableColumns(IEnumerable<TableColumn> tableColumns)
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

        public void CreateContentTable(string tableName, List<TableColumn> tableColumns)
        {
            var isDbExists = _db.IsTableExists(tableName);
            if (isDbExists) return;

            _db.CreateTable(tableName, tableColumns);
            _db.CreateIndex(tableName, $"IX_{tableName}", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");
            _db.CreateIndex(tableName, $"IX_{tableName}_Taxis", ContentAttribute.Taxis);

            ClearCache();
        }

        public void AlterSystemTable(string tableName, List<TableColumn> tableColumns, List<string> dropColumnNames = null)
        {
            _db.AlterTable(tableName, tableColumns, dropColumnNames);

            ClearCache();
        }
    }

}
