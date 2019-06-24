using System;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Repositories;
using SS.CMS.Core.Services;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public partial class InstallController : ControllerBase
    {
        private const string Route = "install";
        private const string RouteTryDatabase = "install/actions/tryDatabase";
        private const string RouteTryRedis = "install/actions/tryRedis";
        private const string RouteSaveSettings = "install/actions/saveSettings";

        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;

        public InstallController(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        [AllowAnonymous]
        [HttpGet(Route)]
        public async Task<ActionResult<Environment>> GetEnvironment()
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);

            var isContentRootPathWritable = false;
            var isWebRootPathWritable = false;

            if (!isInstalled)
            {
                try
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, "version.txt");
                    await FileUtils.WriteTextAsync(filePath, _settingsManager.ProductVersion);

                    var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.ContentRootPath);
                    ioPermission.Demand();

                    isContentRootPathWritable = true;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    var filePath = PathUtils.Combine(_settingsManager.WebRootPath, "index.html");
                    await FileUtils.WriteTextAsync(filePath, Constants.Html5Empty);

                    var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, _settingsManager.WebRootPath);
                    ioPermission.Demand();

                    isWebRootPathWritable = true;
                }
                catch
                {
                    // ignored
                }
            }

            return new Environment
            {
                IsInstalled = isInstalled,
                ApiUrl = Request.Host.Value,
                ProductVersion = _settingsManager.ProductVersion,
                ContentRootPath = _settingsManager.ContentRootPath,
                WebRootPath = _settingsManager.WebRootPath,
                TargetFramework = _settingsManager.TargetFramework,
                IsContentRootPathWritable = isContentRootPathWritable,
                IsWebRootPathWritable = isWebRootPathWritable,
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteTryDatabase)]
        public async Task<ActionResult<ResultModel>> TryDatabase([FromBody] DatabaseModel database)
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
            if (isInstalled) return Forbid();

            var databaseType = DatabaseType.Parse(database.DatabaseType);
            var connectionString = await GetConnectionAsync(databaseType, database);
            var db = new Database(databaseType, connectionString);
            var result = await db.IsConnectionWorksAsync();

            return new ResultModel
            {
                IsSuccess = result.IsConnectionWorks,
                ErrorMessage = result.ErrorMessage
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteTryRedis)]
        public async Task<ActionResult<ResultModel>> TryRedis([FromBody] RedisModel redis)
        {
            var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
            if (isInstalled) return Forbid();

            var (isConnectionWorks, errorMessage) = await _cacheManager.IsRedisConnectionWorksAsync(redis.RedisConnectionString);

            return new ResultModel
            {
                IsSuccess = isConnectionWorks,
                ErrorMessage = errorMessage
            };
        }

        [AllowAnonymous]
        [HttpPost(RouteSaveSettings)]
        public async Task<ActionResult<ResultModel>> SaveSettings([FromBody] SettingsModel settings)
        {
            try
            {
                var isInstalled = !string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString);
                if (isInstalled) return Forbid();

                var databaseType = DatabaseType.Parse(settings.DatabaseType);
                var databaseConnectionString = await GetConnectionAsync(databaseType, settings);

                _settingsManager.SaveSettings(settings.AdminUrl, settings.HomeUrl, settings.Language, settings.IsProtectData, databaseType, databaseConnectionString, settings.RedisIsEnabled, settings.RedisConnectionString);
                // _tableManager.InstallDatabase(settings.AdminName, settings.AdminPassword);

                var accessTokenRepository = new AccessTokenRepository(_settingsManager, _cacheManager);
                var userRoleRepository = new UserRoleRepository(_settingsManager);
                var areaRepository = new AreaRepository(_settingsManager, _cacheManager);
                var channelGroupRepository = new ChannelGroupRepository(_settingsManager, _cacheManager);
                var configRepository = new ConfigRepository(_settingsManager, _cacheManager);
                var contentCheckRepository = new ContentCheckRepository(_settingsManager);
                var contentGroupRepository = new ContentGroupRepository(_settingsManager, _cacheManager);
                var dbCacheRepository = new DbCacheRepository(_settingsManager);
                var departmentRepository = new DepartmentRepository(_settingsManager, _cacheManager);
                var errorLogRepository = new ErrorLogRepository(_settingsManager, configRepository);
                var logRepository = new LogRepository(_settingsManager, configRepository, errorLogRepository);
                var pluginConfigRepository = new PluginConfigRepository(_settingsManager);
                var pluginRepository = new PluginRepository(_settingsManager, _cacheManager);
                var relatedFieldItemRepository = new RelatedFieldItemRepository(_settingsManager);
                var relatedFieldRepository = new RelatedFieldRepository(_settingsManager);
                var roleRepository = new RoleRepository(_settingsManager);
                var siteLogRepository = new SiteLogRepository(_settingsManager, configRepository, errorLogRepository, logRepository);
                var siteRepository = new SiteRepository(_settingsManager, _cacheManager);
                var specialRepository = new SpecialRepository(_settingsManager, _cacheManager);
                var tableStyleItemRepository = new TableStyleItemRepository(_settingsManager);
                var tagRepository = new TagRepository(_settingsManager, _cacheManager);
                var templateLogRepository = new TemplateLogRepository(_settingsManager);
                var userGroupRepository = new UserGroupRepository(_settingsManager, _cacheManager, configRepository);
                var userLogRepository = new UserLogRepository(_settingsManager, configRepository);
                var userMenuRepository = new UserMenuRepository(_settingsManager, _cacheManager);
                var userRepository = new UserRepository(_settingsManager, _cacheManager, configRepository, userRoleRepository);
                var permissionRepository = new PermissionRepository(_settingsManager, _cacheManager, roleRepository);
                var channelRepository = new ChannelRepository(_settingsManager, _cacheManager, userRepository, channelGroupRepository, siteRepository);
                var templateRepository = new TemplateRepository(_settingsManager, _cacheManager, siteRepository, channelRepository, templateLogRepository);
                var tableStyleRepository = new TableStyleRepository(_settingsManager, _cacheManager, siteRepository, channelRepository, userRepository, tableStyleItemRepository, errorLogRepository);

                var tableManager = new TableManager(
                    _settingsManager,
                    _cacheManager,
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

                tableManager.SyncDatabase();

                var userInfo = new UserInfo
                {
                    UserName = settings.AdminName,
                    Password = settings.AdminPassword
                };

                var (userId, errorMessage) = await userRepository.InsertAsync(userInfo);
                userRoleRepository.AddUserToRole(settings.AdminName, AuthTypes.Roles.SuperAdministrator);

                if (userId <= 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ErrorMessage = errorMessage
                    };
                }

                var path = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigFileName);

                var databaseConnectionStringValue = databaseConnectionString;
                var redisConnectionStringValue = settings.RedisConnectionString;
                if (settings.IsProtectData)
                {
                    databaseConnectionStringValue = _settingsManager.Encrypt(databaseConnectionStringValue);
                    redisConnectionStringValue = _settingsManager.Encrypt(redisConnectionStringValue);
                }

                var json = $@"{{
  ""AdminUrl"": ""{_settingsManager.AdminUrl}"",
  ""HomeUrl"": ""{_settingsManager.HomeUrl}"",
  ""Language"": ""{_settingsManager.Language}"",
  ""IsNightlyUpdate"": false,
  ""IsProtectData"": {_settingsManager.IsProtectData.ToString().ToLower()},
  ""SecretKey"": ""{_settingsManager.SecretKey}"",
  ""Database"": {{
    ""Type"": ""{databaseType.Value}"",
    ""ConnectionString"": ""{databaseConnectionStringValue}""
  }},
  ""Redis"": {{
    ""IsEnabled"": {_settingsManager.RedisIsEnabled.ToString().ToLower()},
    ""ConnectionString"": ""{redisConnectionStringValue}""
  }}
}}
";

                await FileUtils.WriteTextAsync(path, json);

                return new ResultModel
                {
                    IsSuccess = true,
                    ErrorMessage = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}