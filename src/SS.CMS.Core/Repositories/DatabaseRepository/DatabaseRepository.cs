using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class DatabaseRepository : IDatabaseRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IUserRepository _userRepository;

        private readonly IDatabase _database;

        public DatabaseRepository(IDistributedCache cache, ISettingsManager settingsManager, IConfigRepository configRepository, IErrorLogRepository errorLogRepository, IUserRepository userRepository)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(DatabaseRepository));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _errorLogRepository = errorLogRepository;
            _userRepository = userRepository;
            _database = new Database(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);
        }

        public IDatabase Database => _database;

        private async Task<IEnumerable<TableColumn>> CacheGetTableColumnInfoListAsync(string tableName)
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                return await _database.GetTableColumnsAsync(tableName);
            });
        }

        public async Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName)
        {
            return await CacheGetTableColumnInfoListAsync(tableName);
        }

        public async Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName, List<string> excludeAttributeNameList)
        {
            var list = await CacheGetTableColumnInfoListAsync(tableName);
            if (excludeAttributeNameList == null || excludeAttributeNameList.Count == 0) return list;

            return list.Where(tableColumnInfo =>
                !StringUtils.ContainsIgnoreCase(excludeAttributeNameList, tableColumnInfo.AttributeName)).ToList();
        }

        public async Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName, DataType excludeDataType)
        {
            var list = await CacheGetTableColumnInfoListAsync(tableName);

            return list.Where(tableColumnInfo =>
                tableColumnInfo.DataType != excludeDataType).ToList();
        }

        public async Task<TableColumn> GetTableColumnInfoAsync(string tableName, string attributeName)
        {
            var list = await CacheGetTableColumnInfoListAsync(tableName);
            return list.FirstOrDefault(tableColumnInfo =>
                StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, attributeName));
        }

        public async Task<bool> IsAttributeNameExistsAsync(string tableName, string attributeName)
        {
            var list = await CacheGetTableColumnInfoListAsync(tableName);
            return list.Any(tableColumnInfo =>
                StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, attributeName));
        }

        public async Task<List<string>> GetTableColumnNameListAsync(string tableName)
        {
            var allTableColumnInfoList = await GetTableColumnInfoListAsync(tableName);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public async Task<List<string>> GetTableColumnNameListAsync(string tableName, List<string> excludeAttributeNameList)
        {
            var allTableColumnInfoList = await GetTableColumnInfoListAsync(tableName, excludeAttributeNameList);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public async Task<List<string>> GetTableColumnNameListAsync(string tableName, DataType excludeDataType)
        {
            var allTableColumnInfoList = await GetTableColumnInfoListAsync(tableName, excludeDataType);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        private IList<TableColumn> GetRealTableColumns(IEnumerable<TableColumn> tableColumns)
        {
            var realTableColumns = new List<TableColumn>();
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Guid)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.CreatedDate)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.LastModifiedDate)))
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
                    AttributeName = nameof(Entity.CreatedDate),
                    DataType = DataType.DateTime
                },
                new TableColumn
                {
                    AttributeName = nameof(Entity.LastModifiedDate),
                    DataType = DataType.DateTime
                }
            });

            return realTableColumns;
        }

        public List<TableColumn> ContentTableDefaultColumns
        {
            get
            {
                return _database.GetTableColumns<Content>();
            }
        }
    }
}
