using System.Collections.Generic;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public static class DataProvider
    {
        private static DatabaseApi _databaseApi;
        public static DatabaseApi DatabaseApi
        {
            get
            {
                if (_databaseApi != null) return _databaseApi;

                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    _databaseApi = new MySql();
                }

                return _databaseApi;
            }
        }

        private static AccessTokenRepository _accessTokenRepository;
        public static AccessTokenRepository AccessTokenRepository => _accessTokenRepository ??= new AccessTokenRepository();

        private static AdministratorRepository _administratorRepository;
        public static AdministratorRepository AdministratorRepository => _administratorRepository ??= new AdministratorRepository();

        private static AdministratorsInRolesRepository _administratorsInRolesRepository;
        public static AdministratorsInRolesRepository AdministratorsInRolesRepository => _administratorsInRolesRepository ??= new AdministratorsInRolesRepository();

        private static LibraryGroupRepository _libraryGroupRepository;
        public static LibraryGroupRepository LibraryGroupRepository => _libraryGroupRepository ??= new LibraryGroupRepository();

        private static LibraryImageRepository _libraryImageRepository;
        public static LibraryImageRepository LibraryImageRepository => _libraryImageRepository ??= new LibraryImageRepository();

        private static LibraryTextRepository _libraryTextRepository;
        public static LibraryTextRepository LibraryTextRepository => _libraryTextRepository ??= new LibraryTextRepository();

        private static ChannelRepository _channelRepository;
        public static ChannelRepository ChannelRepository => _channelRepository ??= new ChannelRepository();

        private static ChannelGroupRepository _channelGroupRepository;
        public static ChannelGroupRepository ChannelGroupRepository => _channelGroupRepository ??= new ChannelGroupRepository();

        private static ConfigRepository _configRepository;
        public static ConfigRepository ConfigRepository => _configRepository ??= new ConfigRepository();

        private static ContentCheckRepository _contentCheckRepository;
        public static ContentCheckRepository ContentCheckRepository => _contentCheckRepository ??= new ContentCheckRepository();

        private static ContentRepository _contentRepository;
        public static ContentRepository ContentRepository => _contentRepository ??= new ContentRepository();

        private static ContentGroupRepository _contentGroupRepository;
        public static ContentGroupRepository ContentGroupRepository => _contentGroupRepository ??= new ContentGroupRepository();

        private static DatabaseRepository _databaseRepository;
        public static DatabaseRepository DatabaseRepository => _databaseRepository ??= new DatabaseRepository();

        private static DbCacheRepository _dbCacheRepository;
        public static DbCacheRepository DbCacheRepository => _dbCacheRepository ??= new DbCacheRepository();

        private static ErrorLogRepository _errorLogRepository;
        public static ErrorLogRepository ErrorLogRepository => _errorLogRepository ??= new ErrorLogRepository();

        private static LogRepository _logRepository;
        public static LogRepository LogRepository => _logRepository ??= new LogRepository();

        private static PermissionsInRolesRepository _permissionsInRolesRepository;
        public static PermissionsInRolesRepository PermissionsInRolesRepository => _permissionsInRolesRepository ??= new PermissionsInRolesRepository();

        private static PluginConfigRepository _pluginConfigRepository;
        public static PluginConfigRepository PluginConfigRepository => _pluginConfigRepository ??= new PluginConfigRepository();

        private static PluginRepository _pluginRepository;
        public static PluginRepository PluginRepository => _pluginRepository ??= new PluginRepository();

        private static RelatedFieldRepository _relatedFieldRepository;
        public static RelatedFieldRepository RelatedFieldRepository => _relatedFieldRepository ??= new RelatedFieldRepository();

        private static RelatedFieldItemRepository _relatedFieldItemRepository;
        public static RelatedFieldItemRepository RelatedFieldItemRepository => _relatedFieldItemRepository ??= new RelatedFieldItemRepository();

        private static RoleRepository _roleRepository;
        public static RoleRepository RoleRepository => _roleRepository ??= new RoleRepository();

        private static SiteRepository _siteRepository;
        public static SiteRepository SiteRepository => _siteRepository ??= new SiteRepository();

        private static SiteLogRepository _siteLogRepository;
        public static SiteLogRepository SiteLogRepository => _siteLogRepository ??= new SiteLogRepository();

        private static SitePermissionsRepository _sitePermissionsRepository;
        public static SitePermissionsRepository SitePermissionsRepository => _sitePermissionsRepository ??= new SitePermissionsRepository();

        private static SpecialRepository _specialRepository;
        public static SpecialRepository SpecialRepository => _specialRepository ??= new SpecialRepository();

        private static TableStyleRepository _tableStyleRepository;
        public static TableStyleRepository TableStyleRepository => _tableStyleRepository ??= new TableStyleRepository();

        private static ContentTagRepository _contentTagRepository;
        public static ContentTagRepository ContentTagRepository => _contentTagRepository ??= new ContentTagRepository();

        private static TemplateRepository _templateRepository;
        public static TemplateRepository TemplateRepository => _templateRepository ??= new TemplateRepository();

        private static TemplateLogRepository _templateLogRepository;
        public static TemplateLogRepository TemplateLogRepository => _templateLogRepository ??= new TemplateLogRepository();

        private static UserRepository _userRepository;
        public static UserRepository UserRepository => _userRepository ??= new UserRepository();

        private static UserGroupRepository _userGroupRepository;
        public static UserGroupRepository UserGroupRepository => _userGroupRepository ??= new UserGroupRepository();

        private static UserLogRepository _userLogRepository;
        public static UserLogRepository UserLogRepository => _userLogRepository ??= new UserLogRepository();

        private static UserMenuRepository _userMenuRepository;
        public static UserMenuRepository UserMenuRepository => _userMenuRepository ??= new UserMenuRepository();

        public static void Reset()
        {
            _databaseApi = null;

            _accessTokenRepository = null;
            _administratorRepository = null;
            _administratorsInRolesRepository = null;
            _libraryGroupRepository = null;
            _libraryImageRepository = null;
            _libraryTextRepository = null;
            _channelRepository = null;
            _channelGroupRepository = null;
            _configRepository = null;
            _contentCheckRepository = null;
            _contentRepository = null;
            _contentGroupRepository = null;
            _databaseRepository = null;
            _dbCacheRepository = null;
            _errorLogRepository = null;
            _logRepository = null;
            _permissionsInRolesRepository = null;
            _pluginConfigRepository = null;
            _pluginRepository = null;
            _relatedFieldRepository = null;
            _relatedFieldItemRepository = null;
            _roleRepository = null;
            _siteRepository = null;
            _siteLogRepository = null;
            _sitePermissionsRepository = null;
            _specialRepository = null;
            _tableStyleRepository = null;
            _contentTagRepository = null;
            _templateRepository = null;
            _templateLogRepository = null;
            _userRepository = null;
            _userGroupRepository = null;
            _userLogRepository = null;
            _userMenuRepository = null;
        }

        public static List<IRepository> AllProviders => new List<IRepository>
        {
            AccessTokenRepository,
            AdministratorRepository,
            AdministratorsInRolesRepository,
            LibraryGroupRepository,
            LibraryImageRepository,
            LibraryTextRepository,
            ChannelRepository,
            ChannelGroupRepository,
            ConfigRepository,
            ContentCheckRepository,
            ContentGroupRepository,
            DbCacheRepository,
            ErrorLogRepository,
            LogRepository,
            PermissionsInRolesRepository,
            PluginConfigRepository,
            PluginRepository,
            RelatedFieldRepository,
            RelatedFieldItemRepository,
            RoleRepository,
            SiteRepository,
            SiteLogRepository,
            SitePermissionsRepository,
            SpecialRepository,
            TableStyleRepository,
            ContentTagRepository,
            TemplateRepository,
            TemplateLogRepository,
            UserRepository,
            UserGroupRepository,
            UserLogRepository,
            UserMenuRepository
        };
    }
}
