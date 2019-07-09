using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common;
using SS.CMS.Core.Repositories;
using SS.CMS.Core.Services;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Cms
{
    public partial class CmsController
    {
        [AllowAnonymous]
        [HttpPost(RouteInstallTryDatabase)]
        public async Task<ActionResult<TryDatabaseResponse>> TryDatabase(TryDatabaseRequest database)
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
            if (isInstalled) return Forbid();

            var databaseType = DatabaseType.Parse(database.DatabaseType);
            var connectionString = database.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = GetDatabaseConnectionString(databaseType, database.Server, database.Port, database.Uid, database.Pwd);
            }

            IDatabase db = new Database(databaseType, connectionString);
            var (isConnectionWorks, errorMessage) = db.IsConnectionWorks();

            IList<string> databaseNames = null;
            if (isConnectionWorks)
            {
                databaseNames = await db.GetDatabaseNamesAsync();
            }

            return new TryDatabaseResponse
            {
                IsSuccess = isConnectionWorks,
                DatabaseNames = databaseNames,
                ErrorMessage = errorMessage
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteInstallTryCache)]
        public async Task<ActionResult<TryCacheResponse>> TryCache(TryCacheRequest cache)
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
            if (isInstalled) return Forbid();

            var isConnectionWorks = true;
            IList<string> databaseNames = null;
            var errorMessage = string.Empty;

            var cacheType = CacheType.Parse(cache.CacheType);

            if (cacheType == CacheType.SqlServer)
            {
                var connectionString = cache.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = GetDatabaseConnectionString(DatabaseType.SqlServer, cache.Server, cache.Port, cache.Uid, cache.Pwd);
                }
                IDatabase db = new Database(DatabaseType.SqlServer, connectionString);

                (isConnectionWorks, errorMessage) = db.IsConnectionWorks();

                if (isConnectionWorks)
                {
                    databaseNames = await db.GetDatabaseNamesAsync();
                }
            }
            else if (cacheType == CacheType.Redis)
            {
                var connectionString = cache.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = GetRedisConnectionString(cache.Server, cache.Port, cache.Pwd);
                }

                (isConnectionWorks, errorMessage) = await RedisManager.IsConnectionWorksAsync(connectionString);
            }

            return new TryCacheResponse
            {
                IsSuccess = isConnectionWorks,
                DatabaseNames = databaseNames,
                ErrorMessage = errorMessage
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteInstallSaveSettings)]
        public async Task<ActionResult<SaveSettingsResponse>> InstallSaveSettings(SaveSettingsRequest settings)
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
            if (isInstalled) return Forbid();

            var databaseType = DatabaseType.Parse(settings.DatabaseType);
            var databaseConnectionString = settings.DatabaseConnectionString;
            if (string.IsNullOrEmpty(databaseConnectionString))
            {
                databaseConnectionString = GetDatabaseConnectionString(databaseType, settings.DatabaseServer, settings.DatabasePort, settings.DatabaseUid, settings.DatabasePwd, settings.DatabaseName);
            }
            var cacheType = CacheType.Parse(settings.CacheType);
            var cacheConnectionString = settings.CacheConnectionString;
            if (string.IsNullOrEmpty(cacheConnectionString))
            {
                if (cacheType == CacheType.SqlServer)
                {
                    cacheConnectionString = GetDatabaseConnectionString(DatabaseType.SqlServer, settings.CacheServer, settings.CachePort, settings.CacheUid, settings.CachePwd);
                }
                else if (cacheType == CacheType.Redis)
                {
                    cacheConnectionString = GetRedisConnectionString(settings.CacheServer, settings.CachePort, settings.CachePwd);
                }
            }

            var securityKey = StringUtils.GetShortGuid().ToLower();

            await _settingsManager.SaveSettingsAsync(false, settings.IsProtectData, securityKey, databaseType, databaseConnectionString, cacheType, cacheConnectionString);

            // _services.AddDistributedCache(cacheType, cacheConnectionString);

            return new SaveSettingsResponse
            {
                SecurityKey = securityKey
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteInstall)]
        public async Task<ActionResult<InstallResponse>> Install(InstallRequest install)
        {
            if (install.SecurityKey != _settingsManager.SecurityKey) return Forbid();

            var database = new Database(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);

            var accessTokenRepository = new AccessTokenRepository(_cache, _settingsManager);
            var userRoleRepository = new UserRoleRepository(_settingsManager);
            var areaRepository = new AreaRepository(_cache, _settingsManager);
            var channelGroupRepository = new ChannelGroupRepository(_cache, _settingsManager);
            var configRepository = new ConfigRepository(_cache, _settingsManager);
            var contentCheckRepository = new ContentCheckRepository(_settingsManager);
            var contentGroupRepository = new ContentGroupRepository(_cache, _settingsManager);
            var dbCacheRepository = new DbCacheRepository(_settingsManager);
            var departmentRepository = new DepartmentRepository(_cache, _settingsManager);
            var errorLogRepository = new ErrorLogRepository(_settingsManager, configRepository);
            var logRepository = new LogRepository(_settingsManager, configRepository, errorLogRepository);
            var pluginConfigRepository = new PluginConfigRepository(_settingsManager);
            var pluginRepository = new PluginRepository(_settingsManager);
            var relatedFieldItemRepository = new RelatedFieldItemRepository(_settingsManager);
            var relatedFieldRepository = new RelatedFieldRepository(_settingsManager);
            var roleRepository = new RoleRepository(_settingsManager);
            var siteLogRepository = new SiteLogRepository(_settingsManager, configRepository, errorLogRepository, logRepository);
            var siteRepository = new SiteRepository(_cache, _settingsManager);
            var specialRepository = new SpecialRepository(_cache, _settingsManager);
            var tableStyleItemRepository = new TableStyleItemRepository(_settingsManager);
            var tagRepository = new TagRepository(_cache, _settingsManager);
            var templateLogRepository = new TemplateLogRepository(_settingsManager);
            var userGroupRepository = new UserGroupRepository(_cache, _settingsManager, configRepository);
            var userLogRepository = new UserLogRepository(_settingsManager, configRepository);
            var userMenuRepository = new UserMenuRepository(_cache, _settingsManager);
            var userRepository = new UserRepository(_cache, _settingsManager, configRepository, userRoleRepository);
            var permissionRepository = new PermissionRepository(_settingsManager, roleRepository);
            var channelRepository = new ChannelRepository(_cache, _settingsManager, userRepository, channelGroupRepository, siteRepository);
            var templateRepository = new TemplateRepository(_cache, _settingsManager, siteRepository, channelRepository, templateLogRepository);
            var tableStyleRepository = new TableStyleRepository(_cache, _settingsManager, siteRepository, channelRepository, userRepository, tableStyleItemRepository, errorLogRepository);

            var tableManager = new TableManager(
                _cache,
                database,
                _settingsManager,
                accessTokenRepository,
                areaRepository,
                channelGroupRepository,
                channelRepository,
                configRepository,
                contentCheckRepository,
                contentGroupRepository,
                dbCacheRepository,
                departmentRepository,
                errorLogRepository,
                logRepository,
                permissionRepository,
                pluginConfigRepository,
                pluginRepository,
                relatedFieldItemRepository,
                relatedFieldRepository,
                roleRepository,
                siteLogRepository,
                siteRepository,
                specialRepository,
                tableStyleItemRepository,
                tableStyleRepository,
                tagRepository,
                templateLogRepository,
                templateRepository,
                userGroupRepository,
                userLogRepository,
                userMenuRepository,
                userRepository,
                userRoleRepository);

            // await tableManager.SyncDatabaseAsync();

            // var configInfo = new ConfigInfo
            // {
            //     DatabaseVersion = _settingsManager.ProductVersion,
            //     UpdateDate = DateTime.UtcNow,
            //     ExtendValues = string.Empty
            // };
            // await _configRepository.DeleteAllAsync();
            // await _configRepository.InsertAsync(configInfo);

            // var userInfo = new UserInfo
            // {
            //     UserName = install.AdminName,
            //     Password = install.AdminPassword,
            //     RoleName = AuthTypes.Roles.SuperAdministrator
            // };

            // var (isSuccess, userId, errorMessage) = await userRepository.InsertAsync(userInfo);

            var (isSuccess, errorMessage) = await tableManager.InstallDatabaseAsync(install.AdminName, install.AdminPassword);

            // var path = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigFileName);

            // var databaseConnectionStringValue = databaseConnectionString;
            // var cacheConnectionStringValue = cacheConnectionString;
            // if (install.IsProtectData)
            // {
            //     databaseConnectionStringValue = _settingsManager.Encrypt(databaseConnectionStringValue);
            //     cacheConnectionStringValue = _settingsManager.Encrypt(cacheConnectionStringValue);
            // }

            return new InstallResponse
            {
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            };
        }
    }
}