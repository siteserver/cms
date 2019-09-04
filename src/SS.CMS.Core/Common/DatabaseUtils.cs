using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Repositories;
using SS.CMS.Data;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Common
{
    public static class DatabaseUtils
    {
        public static (IDatabaseRepository DatabaseRepository, IList<IRepository>) GetAllRepositories(IDistributedCache cache, ISettingsManager settingsManager)
        {
            var accessTokenRepository = new AccessTokenRepository(cache, settingsManager);
            var userRoleRepository = new UserRoleRepository(settingsManager);
            var areaRepository = new AreaRepository(cache, settingsManager);
            var channelGroupRepository = new ChannelGroupRepository(cache, settingsManager);
            var configRepository = new ConfigRepository(cache, settingsManager);
            var contentCheckRepository = new ContentCheckRepository(settingsManager);
            var contentGroupRepository = new ContentGroupRepository(cache, settingsManager);
            var dbCacheRepository = new DbCacheRepository(settingsManager);
            var departmentRepository = new DepartmentRepository(cache, settingsManager);
            var errorLogRepository = new ErrorLogRepository(settingsManager, configRepository);
            var logRepository = new LogRepository(settingsManager, configRepository, errorLogRepository);
            var pluginConfigRepository = new PluginConfigRepository(settingsManager);
            var pluginRepository = new PluginRepository(settingsManager);
            var relatedFieldItemRepository = new RelatedFieldItemRepository(settingsManager);
            var relatedFieldRepository = new RelatedFieldRepository(settingsManager);
            var roleRepository = new RoleRepository(settingsManager);
            var siteLogRepository = new SiteLogRepository(settingsManager, configRepository, errorLogRepository, logRepository);
            var siteRepository = new SiteRepository(cache, settingsManager);
            var specialRepository = new SpecialRepository(cache, settingsManager);
            var tableStyleItemRepository = new TableStyleItemRepository(settingsManager);
            var tagRepository = new TagRepository(cache, settingsManager);
            var templateLogRepository = new TemplateLogRepository(settingsManager);
            var userGroupRepository = new UserGroupRepository(cache, settingsManager, configRepository);
            var userLogRepository = new UserLogRepository(settingsManager, configRepository);
            var userMenuRepository = new UserMenuRepository(cache, settingsManager);
            var userRepository = new UserRepository(cache, settingsManager, configRepository, userRoleRepository);
            var permissionRepository = new PermissionRepository(settingsManager, roleRepository);

            var databaseRepository = new DatabaseRepository(cache, settingsManager, configRepository, errorLogRepository, userRepository);

            var channelRepository = new ChannelRepository(cache, settingsManager, databaseRepository, contentCheckRepository, userRepository, siteRepository, channelGroupRepository, tagRepository, errorLogRepository);

            var tableStyleRepository = new TableStyleRepository(cache, settingsManager, databaseRepository, siteRepository, channelRepository, userRepository, tableStyleItemRepository, errorLogRepository);

            var templateRepository = new TemplateRepository(cache, settingsManager, siteRepository, channelRepository, templateLogRepository);

            var repositories = new List<IRepository>();
            repositories.Add(accessTokenRepository);
            repositories.Add(areaRepository);
            repositories.Add(channelGroupRepository);
            repositories.Add(channelRepository);
            repositories.Add(configRepository);
            repositories.Add(contentCheckRepository);
            repositories.Add(contentGroupRepository);
            repositories.Add(dbCacheRepository);
            repositories.Add(departmentRepository);
            repositories.Add(errorLogRepository);
            repositories.Add(logRepository);
            repositories.Add(permissionRepository);
            repositories.Add(pluginConfigRepository);
            repositories.Add(pluginRepository);
            repositories.Add(relatedFieldItemRepository);
            repositories.Add(relatedFieldRepository);
            repositories.Add(roleRepository);
            repositories.Add(siteLogRepository);
            repositories.Add(siteRepository);
            repositories.Add(specialRepository);
            repositories.Add(tableStyleItemRepository);
            repositories.Add(tableStyleRepository);
            repositories.Add(tagRepository);
            repositories.Add(templateLogRepository);
            repositories.Add(templateRepository);
            repositories.Add(userGroupRepository);
            repositories.Add(userLogRepository);
            repositories.Add(userMenuRepository);
            repositories.Add(userRepository);
            repositories.Add(userRoleRepository);

            return (databaseRepository, repositories);
        }
    }
}