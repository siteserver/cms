using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(TableManager));

        private readonly ICacheManager _cacheManager;
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
            ISettingsManager settingsManager,
            ICacheManager cacheManager,
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
            _cacheManager = cacheManager;
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

        private void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
            //FileWatcher.UpdateCacheFile();
        }

        private void CacheUpdate(Dictionary<string, List<TableColumn>> allDict, List<TableColumn> list,
            string key)
        {
            allDict[key] = list;
        }

        private Dictionary<string, List<TableColumn>> CacheGetAllDictionary()
        {
            var allDict = _cacheManager.Get<Dictionary<string, List<TableColumn>>>(CacheKey);
            if (allDict != null) return allDict;

            allDict = new Dictionary<string, List<TableColumn>>();
            _cacheManager.InsertHours(CacheKey, allDict, 24);
            return allDict;
        }

        private IDatabase GetDatabase()
        {
            return new Database(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);
        }

        private List<TableColumn> CacheGetTableColumnInfoList(string tableName)
        {
            var allDict = CacheGetAllDictionary();

            List<TableColumn> list;
            allDict.TryGetValue(tableName, out list);

            if (list != null) return list;

            var database = GetDatabase();
            list = database.GetTableColumns(tableName);
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
            var database = GetDatabase();

            try
            {
                database.CreateTable(tableName, tableColumns);
            }
            catch (Exception e)
            {
                ex = e;
                _errorLogRepository.AddErrorLog(pluginId, ex, string.Empty);
                return false;
            }

            if (isContentTable)
            {
                try
                {
                    database.CreateIndex(tableName, $"IX_{tableName}_General", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");


                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_General")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.IsTop)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC, {DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Id)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception e)
                {
                    ex = e;
                    _errorLogRepository.AddErrorLog(pluginId, ex, string.Empty);
                    return false;
                }

                try
                {
                    database.CreateIndex(tableName, $"IX_{tableName}_Taxis", $"{ContentAttribute.Taxis} DESC");

                    //sqlString =
                    //    $@"CREATE INDEX {DatorySql.GetQuotedIdentifier(DatabaseType, $"IX_{tableName}_Taxis")} ON {DatorySql.GetQuotedIdentifier(DatabaseType, tableName)}({DatorySql.GetQuotedIdentifier(DatabaseType, ContentAttribute.Taxis)} DESC)";

                    //ExecuteNonQuery(ConnectionString, sqlString);
                }
                catch (Exception e)
                {
                    ex = e;
                    _errorLogRepository.AddErrorLog(pluginId, ex, string.Empty);
                    return false;
                }
            }

            ClearCache();
            return true;
        }

        public void AlterTable(string tableName, List<TableColumn> tableColumns, string pluginId, List<string> dropColumnNames = null)
        {
            var database = GetDatabase();

            try
            {
                database.AlterTable(tableName,
                    GetRealTableColumns(tableColumns), dropColumnNames);

                ClearCache();
            }
            catch (Exception ex)
            {
                _errorLogRepository.AddErrorLog(pluginId, ex, string.Empty);
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

        public void CreateContentTable(string tableName, List<TableColumn> tableColumns)
        {
            var database = GetDatabase();

            var isDbExists = database.IsTableExists(tableName);
            if (isDbExists) return;

            database.CreateTable(tableName, tableColumns);
            database.CreateIndex(tableName, $"IX_{tableName}", $"{ContentAttribute.IsTop} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");
            database.CreateIndex(tableName, $"IX_{tableName}_Taxis", ContentAttribute.Taxis);

            ClearCache();
        }

        public void AlterSystemTable(string tableName, List<TableColumn> tableColumns, List<string> dropColumnNames = null)
        {
            var database = GetDatabase();
            database.AlterTable(tableName, tableColumns, dropColumnNames);

            ClearCache();
        }

        public void InstallDatabase(string userName, string password)
        {
            // SyncDatabase();

            // var userInfo = new UserInfo
            // {
            //     UserName = userName,
            //     Password = password
            // };

            // _userRepository.Insert(userInfo, out _);
            // _userRoleRepository.AddUserToRole(userName, AuthTypes.Roles.SuperAdministrator);
        }

        public void SyncSystemTables()
        {
            var database = GetDatabase();

            if (!database.IsTableExists(_configRepository.TableName))
            {
                CreateTable(_configRepository.TableName, _configRepository.TableColumns, string.Empty, false, out _);
            }
            else
            {
                AlterTable(_configRepository.TableName, _configRepository.TableColumns, string.Empty);
            }

            var configInfo = _configRepository.Instance;
            if (configInfo == null)
            {
                configInfo = new ConfigInfo();
                _configRepository.Insert(configInfo);
            }

            foreach (var repository in _repositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableName == _configRepository.TableName || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!database.IsTableExists(repository.TableName))
                {
                    CreateTable(repository.TableName, repository.TableColumns, string.Empty, false, out _);
                }
                else
                {
                    AlterTable(repository.TableName, repository.TableColumns, string.Empty);
                }
            }
        }

        public void SyncContentTables()
        {
            // var contentDaoList = ContentRepository.GetContentDaoList();
            // foreach (var contentDao in contentDaoList)
            // {
            //     if (!AppContext.Db.IsTableExists(contentDao.TableName))
            //     {
            //         TableColumnManager.CreateTable(contentDao.TableName, contentDao.TableColumns, string.Empty, true, out _);
            //     }
            //     else
            //     {
            //         TableColumnManager.AlterTable(contentDao.TableName, contentDao.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
            //     }
            // }
        }

        public void UpdateConfigVersion()
        {
            var configInfo = _configRepository.Instance;
            if (configInfo == null)
            {
                configInfo = new ConfigInfo
                {
                    DatabaseVersion = _settingsManager.ProductVersion,
                    UpdateDate = DateTime.Now
                };
                _configRepository.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = _settingsManager.ProductVersion;
                configInfo.UpdateDate = DateTime.Now;
                _configRepository.Update(configInfo);
            }
        }

        public void SyncDatabase()
        {
            _cacheManager.ClearAll();

            SyncSystemTables();

            SyncContentTables();

            UpdateConfigVersion();
        }
    }
}
