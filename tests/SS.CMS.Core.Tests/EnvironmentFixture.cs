using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Repositories;
using SS.CMS.Core.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public IConfiguration Configuration { get; }
        public ISettingsManager SettingsManager { get; }
        public ICacheManager CacheManager { get; }
        public IPathManager PathManager { get; }
        public IUrlManager UrlManager { get; }
        public IFileManager FileManager { get; }
        public ICreateManager CreateManager { get; }
        public IPluginManager PluginManager { get; }
        public IAccessTokenRepository AccessTokenRepository { get; }
        public IAdministratorRepository AdministratorRepository { get; }
        public IAdministratorsInRolesRepository AdministratorsInRolesRepository { get; }
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
        public IPermissionsInRolesRepository PermissionsInRolesRepository { get; }
        public IPluginConfigRepository PluginConfigRepository { get; }
        public IPluginRepository PluginRepository { get; }
        public IRelatedFieldItemRepository RelatedFieldItemRepository { get; }
        public IRelatedFieldRepository RelatedFieldRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public ISiteLogRepository SiteLogRepository { get; }
        public ISitePermissionsRepository SitePermissionsRepository { get; }
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

        public EnvironmentFixture()
        {
            var projDirectoryPath = DirectoryUtils.GetParentPath(Directory.GetCurrentDirectory(), 3);

            var config = new ConfigurationBuilder()
                .SetBasePath(projDirectoryPath)
                .AddJsonFile("appSettings.json")
                .Build();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheManager = new CacheManager(memoryCache, null);
            var settingsManager = new SettingsManager(config, projDirectoryPath, PathUtils.Combine(projDirectoryPath, "wwwroot"));

            var accessTokenRepository = new AccessTokenRepository(settingsManager, cacheManager);

            var administratorsInRolesRepository = new AdministratorsInRolesRepository(settingsManager);
            var areaRepository = new AreaRepository(settingsManager);
            var channelGroupRepository = new ChannelGroupRepository(settingsManager);
            var configRepository = new ConfigRepository(settingsManager);
            var contentCheckRepository = new ContentCheckRepository(settingsManager);
            var contentGroupRepository = new ContentGroupRepository(settingsManager, cacheManager);
            var dbCacheRepository = new DbCacheRepository(settingsManager);
            var departmentRepository = new DepartmentRepository(settingsManager, cacheManager);
            var errorLogRepository = new ErrorLogRepository(settingsManager);
            var logRepository = new LogRepository(settingsManager);
            var permissionsInRolesRepository = new PermissionsInRolesRepository(settingsManager);
            var pluginConfigRepository = new PluginConfigRepository(settingsManager);
            var pluginRepository = new PluginRepository(settingsManager, cacheManager);
            var relatedFieldItemRepository = new RelatedFieldItemRepository(settingsManager);
            var relatedFieldRepository = new RelatedFieldRepository(settingsManager);
            var roleRepository = new RoleRepository(settingsManager);
            var siteLogRepository = new SiteLogRepository(settingsManager);
            var sitePermissionsRepository = new SitePermissionsRepository(settingsManager, permissionsInRolesRepository);
            var siteRepository = new SiteRepository(settingsManager);
            var specialRepository = new SpecialRepository(settingsManager, cacheManager);
            var tableStyleItemRepository = new TableStyleItemRepository(settingsManager);
            var tagRepository = new TagRepository(settingsManager);
            var templateLogRepository = new TemplateLogRepository(settingsManager);
            var templateRepository = new TemplateRepository(settingsManager, cacheManager, siteRepository, templateLogRepository);
            var userGroupRepository = new UserGroupRepository(settingsManager, cacheManager);
            var userLogRepository = new UserLogRepository(settingsManager);
            var userMenuRepository = new UserMenuRepository(settingsManager, cacheManager);
            var userRepository = new UserRepository(settingsManager, cacheManager, userLogRepository);

            var tableStyleRepository = new TableStyleRepository(settingsManager, userRepository, tableStyleItemRepository);
            var administratorRepository = new AdministratorRepository(settingsManager, cacheManager, administratorsInRolesRepository, permissionsInRolesRepository, sitePermissionsRepository);
            var channelRepository = new ChannelRepository(settingsManager, administratorRepository, channelGroupRepository, siteRepository, templateRepository);

            var pathManager = new PathManager(settingsManager, siteRepository, templateRepository);
            var pluginManager = new PluginManager(settingsManager, pathManager, pluginRepository, siteRepository, tableStyleRepository);
            var urlManager = new UrlManager(settingsManager, pathManager, pluginManager, siteRepository, specialRepository, templateRepository);
            var fileManager = new FileManager(settingsManager, urlManager, pathManager, pluginManager, siteRepository, templateRepository);
            var createManager = new CreateManager(config, settingsManager, pluginManager, urlManager, pathManager, fileManager, siteRepository, specialRepository, userRepository, tableStyleRepository, templateRepository);

            Configuration = config;
            SettingsManager = settingsManager;
            CacheManager = cacheManager;
            PathManager = pathManager;
            UrlManager = urlManager;
            FileManager = fileManager;
            CreateManager = createManager;
            PluginManager = pluginManager;
            AccessTokenRepository = accessTokenRepository;
            AdministratorRepository = administratorRepository;
            AdministratorsInRolesRepository = administratorsInRolesRepository;
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
            PermissionsInRolesRepository = permissionsInRolesRepository;
            PluginConfigRepository = pluginConfigRepository;
            PluginRepository = pluginRepository;
            RelatedFieldItemRepository = relatedFieldItemRepository;
            RelatedFieldRepository = relatedFieldRepository;
            RoleRepository = roleRepository;
            SiteLogRepository = siteLogRepository;
            SitePermissionsRepository = sitePermissionsRepository;
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
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
