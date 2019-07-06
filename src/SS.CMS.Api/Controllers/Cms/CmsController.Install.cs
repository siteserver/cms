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
        private const string RouteInstallTryDatabase = "install/tryDatabase";
        private const string RouteInstallTryCache = "install/tryCache";
        private const string RouteInstall = "install";

        [AllowAnonymous]
        [HttpPost(RouteInstallTryDatabase)]
        public async Task<ActionResult<ResultResponse>> TryDatabase(DatabaseRequest database)
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
            var result = db.IsConnectionWorks();
            IList<string> databaseNames = null;
            if (result.IsConnectionWorks)
            {
                databaseNames = await db.GetDatabaseNamesAsync();
            }

            return new ResultResponse
            {
                IsSuccess = result.IsConnectionWorks,
                DatabaseNames = databaseNames,
                ErrorMessage = result.ErrorMessage
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteInstallTryCache)]
        public async Task<ActionResult<ResultResponse>> TryCache(CacheRequest cache)
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

            return new ResultResponse
            {
                IsSuccess = isConnectionWorks,
                DatabaseNames = databaseNames,
                ErrorMessage = errorMessage
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteInstall)]
        public async Task<ActionResult<ResultResponse>> Install(InstallRequest install)
        {
            try
            {
                var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
                if (isInstalled) return Forbid();

                var databaseType = DatabaseType.Parse(install.DatabaseType);
                var databaseConnectionString = install.DatabaseConnectionString;
                if (string.IsNullOrEmpty(databaseConnectionString))
                {
                    databaseConnectionString = GetDatabaseConnectionString(databaseType, install.DatabaseServer, install.DatabasePort, install.DatabaseUid, install.DatabasePwd, install.DatabaseName);
                }
                var cacheType = CacheType.Parse(install.CacheType);
                var cacheConnectionString = install.CacheConnectionString;
                if (string.IsNullOrEmpty(cacheConnectionString))
                {
                    if (cacheType == CacheType.SqlServer)
                    {
                        cacheConnectionString = GetDatabaseConnectionString(DatabaseType.SqlServer, install.CacheServer, install.CachePort, install.CacheUid, install.CachePwd);
                    }
                    else if (cacheType == CacheType.Redis)
                    {
                        cacheConnectionString = GetRedisConnectionString(install.CacheServer, install.CachePort, install.CachePwd);
                    }
                }

                var securityKey = StringUtils.GetShortGuid().ToLower();

                await _settingsManager.SaveSettingsAsync(install.AdminUrl, install.HomeUrl, false, install.IsProtectData, securityKey, databaseType, databaseConnectionString, cacheType, cacheConnectionString);
                // _tableManager.InstallDatabase(install.AdminName, install.AdminPassword);

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

                await tableManager.SyncDatabaseAsync();

                var userInfo = new UserInfo
                {
                    UserName = install.AdminName,
                    Password = install.AdminPassword
                };

                var (isSuccess, userId, errorMessage) = await userRepository.InsertAsync(userInfo);
                userRoleRepository.AddUserToRole(install.AdminName, AuthTypes.Roles.SuperAdministrator);

                if (!isSuccess)
                {
                    return new ResultResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = errorMessage
                    };
                }

                var path = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigFileName);

                var databaseConnectionStringValue = databaseConnectionString;
                var cacheConnectionStringValue = cacheConnectionString;
                if (install.IsProtectData)
                {
                    databaseConnectionStringValue = _settingsManager.Encrypt(databaseConnectionStringValue);
                    cacheConnectionStringValue = _settingsManager.Encrypt(cacheConnectionStringValue);
                }

                //                 var json = $@"{{
                //   ""AdminUrl"": ""{_settingsManager.AdminUrl}"",
                //   ""HomeUrl"": ""{_settingsManager.HomeUrl}"",
                //   ""Language"": ""{_settingsManager.Language}"",
                //   ""IsNightlyUpdate"": false,
                //   ""IsProtectData"": {_settingsManager.IsProtectData.ToString().ToLower()},
                //   ""SecretKey"": ""{_settingsManager.SecurityKey}"",
                //   ""Database"": {{
                //     ""Type"": ""{databaseType.Value}"",
                //     ""ConnectionString"": ""{databaseConnectionStringValue}""
                //   }},
                //   ""Cache"": {{
                //     ""Type"": {cacheType.Value},
                //     ""ConnectionString"": ""{cacheConnectionStringValue}""
                //   }}
                // }}
                // ";

                //                 await FileUtils.WriteTextAsync(path, json);

                return new ResultResponse
                {
                    IsSuccess = true,
                    ErrorMessage = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new ResultResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public class DatabaseRequest
        {
            public string DatabaseType { get; set; }
            public string Server { get; set; }
            public int? Port { get; set; }
            public string Uid { get; set; }
            public string Pwd { get; set; }
            public string ConnectionString { get; set; }
        }

        public class CacheRequest
        {
            public string CacheType { get; set; }
            public string Server { get; set; }
            public int? Port { get; set; }
            public string Uid { get; set; }
            public string Pwd { get; set; }
            public string ConnectionString { get; set; }
        }

        public class InstallRequest
        {
            public string DatabaseType { get; set; }
            public string DatabaseServer { get; set; }
            public int? DatabasePort { get; set; }
            public string DatabaseUid { get; set; }
            public string DatabasePwd { get; set; }
            public string DatabaseName { get; set; }
            public string DatabaseConnectionString { get; set; }
            public string CacheType { get; set; }
            public string CacheServer { get; set; }
            public int? CachePort { get; set; }
            public string CacheUid { get; set; }
            public string CachePwd { get; set; }
            public string CacheName { get; set; }
            public string CacheConnectionString { get; set; }
            public string AdminUrl { get; set; }
            public string HomeUrl { get; set; }
            public string AdminName { get; set; }
            public string AdminPassword { get; set; }
            public bool IsProtectData { get; set; }
        }

        public class ResultResponse
        {
            public bool IsSuccess { get; set; }
            public IList<string> DatabaseNames { get; set; }
            public string ErrorMessage { get; set; }
        }

        private string GetDatabaseConnectionString(DatabaseType databaseType, string server, int? port, string uid, string pwd, string databaseName = null)
        {
            if (databaseType == DatabaseType.SQLite)
            {
                var dbFilePath = PathUtils.Combine(_settingsManager.ContentRootPath, "database.sqlite");
                if (!FileUtils.IsFileExists(dbFilePath))
                {
                    FileUtils.WriteText(PathUtils.Combine(_settingsManager.ContentRootPath, "database.sqlite"), string.Empty);
                }

                return "Data Source=~/database.sqlite;Version=3;";
            }

            var connectionString = $"Server={server};";
            if (port.HasValue && port.Value > 0)
            {
                connectionString += $"Port={port.Value};";
            }
            connectionString += $"Uid={uid};Pwd={pwd};";
            if (!string.IsNullOrEmpty(databaseName))
            {
                connectionString += $"Database={databaseName};";
            }
            return connectionString;
        }

        private string GetRedisConnectionString(string server, int? port, string pwd)
        {
            var connectionString = server;
            if (port.HasValue && port.Value > 0)
            {
                connectionString += $":{port.Value}";
            }
            if (!string.IsNullOrEmpty(pwd))
            {
                connectionString += $",password={pwd}";
            }
            return connectionString;
        }
    }
}