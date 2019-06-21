using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.Repositories;
using SS.CMS.Core.Services;
using SS.CMS.Data;
using SS.CMS.Repositories;
using SS.CMS.Services;
using Xunit;

namespace SS.CMS.Utils.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public IConfiguration Configuration { get; }
        public ISettingsManager SettingsManager { get; }
        public ICacheManager CacheManager { get; }
        public IPathManager PathManager { get; }
        public IPluginManager PluginManager { get; }
        public IUrlManager UrlManager { get; }
        public IFileManager FileManager { get; }
        public ICreateManager CreateManager { get; }
        public ITableManager TableManager { get; }

        public IAccessTokenRepository AccessTokenRepository { get; }
        public IAreaRepository AreaRepository { get; }
        public IChannelGroupRepository ChannelGroupRepository { get; }
        public IChannelRepository ChannelRepository { get; }
        public IConfigRepository ConfigRepository { get; }
        public IContentCheckRepository ContentCheckRepository { get; }
        public IContentGroupRepository ContentGroupRepository { get; }
        public IDbCacheRepository DbCacheRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }
        public ILogRepository LogRepository { get; }
        public IPermissionRepository PermissionRepository { get; }
        public IPluginConfigRepository PluginConfigRepository { get; }
        public IPluginRepository PluginRepository { get; }
        public IRelatedFieldItemRepository RelatedFieldItemRepository { get; }
        public IRelatedFieldRepository RelatedFieldRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public ISiteLogRepository SiteLogRepository { get; }
        public ISiteRepository SiteRepository { get; }
        public ISpecialRepository SpecialRepository { get; }
        public ITableStyleItemRepository TableStyleItemRepository { get; }
        public ITableStyleRepository TableStyleRepository { get; }
        public ITagRepository TagRepository { get; }
        public ITemplateLogRepository TemplateLogRepository { get; }
        public ITemplateRepository TemplateRepository { get; }
        public IUserGroupRepository UserGroupRepository { get; }
        public IUserLogRepository UserLogRepository { get; }
        public IUserMenuRepository UserMenuRepository { get; }
        public IUserRepository UserRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }

        public DatabaseFixture()
        {
            var contentRootPath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("appSettings.json")
                .Build();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheManager = new CacheManager(memoryCache, null);
            var settingsManager = new SettingsManager(config, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"));

            var accessTokenRepository = new AccessTokenRepository(settingsManager, cacheManager);

            var userRoleRepository = new UserRoleRepository(settingsManager);
            var areaRepository = new AreaRepository(settingsManager, cacheManager);
            var channelGroupRepository = new ChannelGroupRepository(settingsManager, cacheManager);
            var configRepository = new ConfigRepository(settingsManager, cacheManager);
            var contentCheckRepository = new ContentCheckRepository(settingsManager);
            var contentGroupRepository = new ContentGroupRepository(settingsManager, cacheManager);
            var dbCacheRepository = new DbCacheRepository(settingsManager);
            var departmentRepository = new DepartmentRepository(settingsManager, cacheManager);
            var errorLogRepository = new ErrorLogRepository(settingsManager, configRepository);
            var logRepository = new LogRepository(settingsManager, configRepository, errorLogRepository);
            var pluginConfigRepository = new PluginConfigRepository(settingsManager);
            var pluginRepository = new PluginRepository(settingsManager, cacheManager);
            var relatedFieldItemRepository = new RelatedFieldItemRepository(settingsManager);
            var relatedFieldRepository = new RelatedFieldRepository(settingsManager);
            var roleRepository = new RoleRepository(settingsManager);
            var siteLogRepository = new SiteLogRepository(settingsManager, configRepository, errorLogRepository, logRepository);
            var siteRepository = new SiteRepository(settingsManager, cacheManager);
            var specialRepository = new SpecialRepository(settingsManager, cacheManager);
            var tableStyleItemRepository = new TableStyleItemRepository(settingsManager);
            var tagRepository = new TagRepository(settingsManager, cacheManager);
            var templateLogRepository = new TemplateLogRepository(settingsManager);

            var userGroupRepository = new UserGroupRepository(settingsManager, cacheManager, configRepository);
            var userLogRepository = new UserLogRepository(settingsManager, configRepository);
            var userMenuRepository = new UserMenuRepository(settingsManager, cacheManager);
            var userRepository = new UserRepository(settingsManager, cacheManager, configRepository, userRoleRepository);

            var permissionRepository = new PermissionRepository(settingsManager, cacheManager, roleRepository);
            var channelRepository = new ChannelRepository(settingsManager, cacheManager, userRepository, channelGroupRepository, siteRepository);
            var templateRepository = new TemplateRepository(settingsManager, cacheManager, siteRepository, channelRepository, templateLogRepository);
            var tableStyleRepository = new TableStyleRepository(settingsManager, cacheManager, siteRepository, channelRepository, userRepository, tableStyleItemRepository, errorLogRepository);

            var tableManager = new TableManager(
                settingsManager,
                cacheManager,
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

            var pathManager = new PathManager(settingsManager, tableManager, siteRepository, channelRepository, templateRepository);
            var pluginManager = new PluginManager(settingsManager, cacheManager, pathManager, tableManager, pluginRepository, siteRepository, channelRepository, tableStyleRepository, errorLogRepository);
            var urlManager = new UrlManager(settingsManager, pathManager, pluginManager, configRepository, siteRepository, channelRepository, specialRepository, templateRepository, errorLogRepository);
            var fileManager = new FileManager(settingsManager, urlManager, pathManager, pluginManager, siteRepository, channelRepository, templateRepository, tagRepository, errorLogRepository);
            var createManager = new CreateManager(config, settingsManager, cacheManager, pluginManager, urlManager, pathManager, fileManager, tableManager, siteRepository, channelRepository, specialRepository, userRepository, tableStyleRepository, templateRepository, tagRepository, errorLogRepository);

            Configuration = config;
            SettingsManager = settingsManager;
            CacheManager = cacheManager;
            PathManager = pathManager;
            UrlManager = urlManager;
            FileManager = fileManager;
            CreateManager = createManager;
            PluginManager = pluginManager;
            TableManager = tableManager;
            AccessTokenRepository = accessTokenRepository;
            UserRoleRepository = userRoleRepository;
            AreaRepository = areaRepository;
            ChannelGroupRepository = channelGroupRepository;
            ChannelRepository = channelRepository;
            ConfigRepository = configRepository;
            ContentCheckRepository = contentCheckRepository;
            ContentGroupRepository = contentGroupRepository;
            DbCacheRepository = dbCacheRepository;
            DepartmentRepository = departmentRepository;
            ErrorLogRepository = errorLogRepository;
            LogRepository = logRepository;
            PermissionRepository = permissionRepository;
            PluginConfigRepository = pluginConfigRepository;
            PluginRepository = pluginRepository;
            RelatedFieldItemRepository = relatedFieldItemRepository;
            RelatedFieldRepository = relatedFieldRepository;
            RoleRepository = roleRepository;
            SiteLogRepository = siteLogRepository;
            SiteRepository = siteRepository;
            SpecialRepository = specialRepository;
            TableStyleItemRepository = tableStyleItemRepository;
            TableStyleRepository = tableStyleRepository;
            TagRepository = tagRepository;
            TemplateLogRepository = templateLogRepository;
            TemplateRepository = templateRepository;
            UserGroupRepository = userGroupRepository;
            UserLogRepository = userLogRepository;
            UserMenuRepository = userMenuRepository;
            UserRepository = userRepository;

            var db = new Database(SettingsManager.DatabaseType, SettingsManager.DatabaseConnectionString);
            var tableNames = db.GetTableNames();
            foreach (var tableName in tableNames)
            {
                db.DropTable(tableName);
            }

            tableManager.InstallDatabase("admin", "admin888");
        }

        public void Dispose()
        {
            var db = new Database(SettingsManager.DatabaseType, SettingsManager.DatabaseConnectionString);
            var tableNames = db.GetTableNames();
            foreach (var tableName in tableNames)
            {
                db.DropTable(tableName);
            }
            // ... clean up test data from the database ...
        }
    }
}
