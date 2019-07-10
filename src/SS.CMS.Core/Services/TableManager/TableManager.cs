using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class TableManager : ITableManager
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly IDatabase _database;
        private readonly ISettingsManager _settingsManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IConfigRepository _configRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ILogRepository _logRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPluginConfigRepository _pluginConfigRepository;
        private readonly IPluginRepository _pluginRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteLogRepository _siteLogRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITableStyleItemRepository _tableStyleItemRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITemplateLogRepository _templateLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IList<IRepository> _repositories;

        public TableManager(
            IDistributedCache cache,
            IDatabase database,
            ISettingsManager settingsManager,
            IAccessTokenRepository accessTokenRepository,
            IAreaRepository areaRepository,
            IChannelGroupRepository channelGroupRepository,
            IChannelRepository channelRepository,
            IConfigRepository configRepository,
            IContentCheckRepository contentCheckRepository,
            IContentGroupRepository contentGroupRepository,
            IDbCacheRepository dbCacheRepository,
            IDepartmentRepository departmentRepository,
            IErrorLogRepository errorLogRepository,
            ILogRepository logRepository,
            IPermissionRepository permissionRepository,
            IPluginConfigRepository pluginConfigRepository,
            IPluginRepository pluginRepository,
            IRelatedFieldItemRepository relatedFieldItemRepository,
            IRelatedFieldRepository relatedFieldRepository,
            IRoleRepository roleRepository,
            ISiteLogRepository siteLogRepository,
            ISiteRepository siteRepository,
            ISpecialRepository specialRepository,
            ITableStyleItemRepository tableStyleItemRepository,
            ITableStyleRepository tableStyleRepository,
            ITagRepository tagRepository,
            ITemplateLogRepository templateLogRepository,
            ITemplateRepository templateRepository,
            IUserGroupRepository userGroupRepository,
            IUserLogRepository userLogRepository,
            IUserMenuRepository userMenuRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository
        )
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(TableManager));
            _database = database;
            _settingsManager = settingsManager;

            _repositories = new List<IRepository>();

            _accessTokenRepository = accessTokenRepository;
            _repositories.Add(_accessTokenRepository);
            _areaRepository = areaRepository;
            _repositories.Add(_areaRepository);
            _channelGroupRepository = channelGroupRepository;
            _repositories.Add(_channelGroupRepository);
            _channelRepository = channelRepository;
            _repositories.Add(_channelRepository);
            _configRepository = configRepository;
            _repositories.Add(_configRepository);
            _contentCheckRepository = contentCheckRepository;
            _repositories.Add(_contentCheckRepository);
            _contentGroupRepository = contentGroupRepository;
            _repositories.Add(_contentGroupRepository);
            _dbCacheRepository = dbCacheRepository;
            _repositories.Add(_dbCacheRepository);
            _departmentRepository = departmentRepository;
            _repositories.Add(_departmentRepository);
            _errorLogRepository = errorLogRepository;
            _repositories.Add(_errorLogRepository);
            _logRepository = logRepository;
            _repositories.Add(_logRepository);
            _permissionRepository = permissionRepository;
            _repositories.Add(_permissionRepository);
            _pluginConfigRepository = pluginConfigRepository;
            _repositories.Add(_pluginConfigRepository);
            _pluginRepository = pluginRepository;
            _repositories.Add(_pluginRepository);
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _repositories.Add(_relatedFieldItemRepository);
            _relatedFieldRepository = relatedFieldRepository;
            _repositories.Add(_relatedFieldRepository);
            _roleRepository = roleRepository;
            _repositories.Add(_roleRepository);
            _siteLogRepository = siteLogRepository;
            _repositories.Add(_siteLogRepository);
            _siteRepository = siteRepository;
            _repositories.Add(_siteRepository);
            _specialRepository = specialRepository;
            _repositories.Add(_specialRepository);
            _tableStyleItemRepository = tableStyleItemRepository;
            _repositories.Add(_tableStyleItemRepository);
            _tableStyleRepository = tableStyleRepository;
            _repositories.Add(_tableStyleRepository);
            _tagRepository = tagRepository;
            _repositories.Add(_tagRepository);
            _templateLogRepository = templateLogRepository;
            _repositories.Add(_templateLogRepository);
            _templateRepository = templateRepository;
            _repositories.Add(_templateRepository);
            _userGroupRepository = userGroupRepository;
            _repositories.Add(_userGroupRepository);
            _userLogRepository = userLogRepository;
            _repositories.Add(_userLogRepository);
            _userMenuRepository = userMenuRepository;
            _repositories.Add(_userMenuRepository);
            _userRepository = userRepository;
            _repositories.Add(_userRepository);
            _userRoleRepository = userRoleRepository;
            _repositories.Add(_userRoleRepository);
        }

        private void CacheUpdate(Dictionary<string, List<TableColumn>> allDict, List<TableColumn> list,
            string key)
        {
            allDict[key] = list;
        }

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

        public async Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable)
        {
            try
            {
                await _database.CreateTableAsync(tableName, tableColumns);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                return (false, ex);
            }

            if (isContentTable)
            {
                try
                {
                    await _database.CreateIndexAsync(tableName, $"IX_{tableName}_General", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }

                try
                {
                    await _database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_Taxis")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
                    return (false, ex);
                }
            }

            await _cache.RemoveAsync(_cacheKey);
            return (true, null);
        }

        public async Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null)
        {
            try
            {
                await _database.AlterTableAsync(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);

                await _cache.RemoveAsync(_cacheKey);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(pluginId, ex, string.Empty);
            }
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

        public async Task AlterSystemTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null)
        {
            await _database.AlterTableAsync(tableName, tableColumns, dropColumnNames);

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> InstallDatabaseAsync(string adminName, string adminPassword)
        {
            // SyncDatabase();

            // var userInfo = new UserInfo
            // {
            //     UserName = userName,
            //     Password = password
            // };

            // _userRepository.Insert(userInfo, out _);
            // _userRoleRepository.AddUserToRole(userName, AuthTypes.Roles.SuperAdministrator);

            await SyncDatabaseAsync();

            var configInfo = new Config
            {
                DatabaseVersion = _settingsManager.ProductVersion,
                UpdateDate = DateTime.UtcNow,
                ExtendValues = string.Empty
            };
            await _configRepository.DeleteAllAsync();
            await _configRepository.InsertAsync(configInfo);

            var userInfo = new User
            {
                UserName = adminName,
                Password = adminPassword,
                RoleName = AuthTypes.Roles.SuperAdministrator
            };

            var (isSuccess, userId, errorMessage) = await _userRepository.InsertAsync(userInfo);

            return (isSuccess, errorMessage);
        }

        public async Task SyncSystemTablesAsync()
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (!await _database.IsTableExistsAsync(_configRepository.TableName))
            {
                await CreateTableAsync(_configRepository.TableName, _configRepository.TableColumns, string.Empty, false);
            }
            else
            {
                await AlterTableAsync(_configRepository.TableName, _configRepository.TableColumns, string.Empty);
            }

            if (configInfo == null)
            {
                configInfo = new Config();
                await _configRepository.InsertAsync(configInfo);
            }

            foreach (var repository in _repositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableName == _configRepository.TableName || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!await _database.IsTableExistsAsync(repository.TableName))
                {
                    await CreateTableAsync(repository.TableName, repository.TableColumns, string.Empty, false);
                }
                else
                {
                    await AlterTableAsync(repository.TableName, repository.TableColumns, string.Empty);
                }
            }
        }

        public async Task UpdateConfigVersionAsync()
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (configInfo != null)
            {
                configInfo.DatabaseVersion = _settingsManager.ProductVersion;
                configInfo.UpdateDate = DateTime.UtcNow;
                await _configRepository.UpdateAsync(configInfo);
            }
        }

        public async Task SyncDatabaseAsync()
        {
            await SyncSystemTablesAsync();

            SyncContentTables();

            await UpdateConfigVersionAsync();
        }
    }
}
