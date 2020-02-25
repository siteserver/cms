using System.Collections.Generic;
using Datory;
using SS.CMS.Repositories;
using SS.CMS.Abstractions;

namespace SS.CMS.Framework
{
    public static class DataProvider
    {
        private static ISettingsManager _settingsManager;

        public static void Load(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        private static IAccessTokenRepository _accessTokenRepository;
        public static IAccessTokenRepository AccessTokenRepository => _accessTokenRepository ??= new AccessTokenRepository(_settingsManager);

        private static IAdministratorsInRolesRepository _administratorsInRolesRepository;
        public static IAdministratorsInRolesRepository AdministratorsInRolesRepository => _administratorsInRolesRepository ??= new AdministratorsInRolesRepository(_settingsManager);

        private static ILibraryGroupRepository _libraryGroupRepository;
        public static ILibraryGroupRepository LibraryGroupRepository => _libraryGroupRepository ??= new LibraryGroupRepository(_settingsManager);

        private static ILibraryImageRepository _libraryImageRepository;
        public static ILibraryImageRepository LibraryImageRepository => _libraryImageRepository ??= new LibraryImageRepository(_settingsManager);

        private static ILibraryVideoRepository _libraryVideoRepository;
        public static ILibraryVideoRepository LibraryVideoRepository => _libraryVideoRepository ??= new LibraryVideoRepository(_settingsManager);

        private static ILibraryFileRepository _libraryFileRepository;
        public static ILibraryFileRepository LibraryFileRepository => _libraryFileRepository ??= new LibraryFileRepository(_settingsManager);

        private static ILibraryTextRepository _libraryTextRepository;
        public static ILibraryTextRepository LibraryTextRepository => _libraryTextRepository ??= new LibraryTextRepository(_settingsManager);

        private static IChannelGroupRepository _channelGroupRepository;
        public static IChannelGroupRepository ChannelGroupRepository => _channelGroupRepository ??= new ChannelGroupRepository(_settingsManager);

        private static IConfigRepository _configRepository;
        public static IConfigRepository ConfigRepository => _configRepository ??= new ConfigRepository(_settingsManager);

        private static IContentCheckRepository _contentCheckRepository;
        public static IContentCheckRepository ContentCheckRepository => _contentCheckRepository ??= new ContentCheckRepository(_settingsManager);

        private static IContentGroupRepository _contentGroupRepository;
        public static IContentGroupRepository ContentGroupRepository => _contentGroupRepository ??= new ContentGroupRepository(_settingsManager);

        private static IDbCacheRepository _dbCacheRepository;
        public static IDbCacheRepository DbCacheRepository => _dbCacheRepository ??= new DbCacheRepository(_settingsManager);

        private static IPermissionsInRolesRepository _permissionsInRolesRepository;
        public static IPermissionsInRolesRepository PermissionsInRolesRepository => _permissionsInRolesRepository ??= new PermissionsInRolesRepository(_settingsManager);

        private static IPluginConfigRepository _pluginConfigRepository;
        public static IPluginConfigRepository PluginConfigRepository => _pluginConfigRepository ??= new PluginConfigRepository(_settingsManager);

        private static IPluginRepository _pluginRepository;
        public static IPluginRepository PluginRepository => _pluginRepository ??= new PluginRepository(_settingsManager);

        private static IRelatedFieldRepository _relatedFieldRepository;
        public static IRelatedFieldRepository RelatedFieldRepository => _relatedFieldRepository ??= new RelatedFieldRepository(_settingsManager);

        private static IRelatedFieldItemRepository _relatedFieldItemRepository;
        public static IRelatedFieldItemRepository RelatedFieldItemRepository => _relatedFieldItemRepository ??= new RelatedFieldItemRepository(_settingsManager);

        private static IRoleRepository _roleRepository;
        public static IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_settingsManager);

        private static ISitePermissionsRepository _sitePermissionsRepository;
        public static ISitePermissionsRepository SitePermissionsRepository => _sitePermissionsRepository ??= new SitePermissionsRepository(_settingsManager);

        private static ISpecialRepository _specialRepository;
        public static ISpecialRepository SpecialRepository => _specialRepository ??= new SpecialRepository(_settingsManager);

        private static ITableStyleRepository _tableStyleRepository;
        public static ITableStyleRepository TableStyleRepository => _tableStyleRepository ??= new TableStyleRepository(_settingsManager);

        private static IContentTagRepository _contentTagRepository;
        public static IContentTagRepository ContentTagRepository => _contentTagRepository ??= new ContentTagRepository(_settingsManager);

        private static IUserMenuRepository _userMenuRepository;
        public static IUserMenuRepository UserMenuRepository => _userMenuRepository ??= new UserMenuRepository(_settingsManager);

        private static IErrorLogRepository _errorLogRepository;
        public static IErrorLogRepository ErrorLogRepository => _errorLogRepository ??= new ErrorLogRepository(_settingsManager, ConfigRepository);

        private static ISiteLogRepository _siteLogRepository;
        public static ISiteLogRepository SiteLogRepository => _siteLogRepository ??= new SiteLogRepository(_settingsManager, ConfigRepository, AdministratorRepository, LogRepository, ErrorLogRepository);

        private static IUserRepository _userRepository;
        public static IUserRepository UserRepository => _userRepository ??= new UserRepository(_settingsManager, ConfigRepository, UserLogRepository);

        private static IUserGroupRepository _userGroupRepository;
        public static IUserGroupRepository UserGroupRepository => _userGroupRepository ??= new UserGroupRepository(_settingsManager, ConfigRepository);

        private static IUserLogRepository _userLogRepository;
        public static IUserLogRepository UserLogRepository => _userLogRepository ??= new UserLogRepository(_settingsManager, ConfigRepository, ErrorLogRepository);

        private static IAdministratorRepository _administratorRepository;
        public static IAdministratorRepository AdministratorRepository => _administratorRepository ??= new AdministratorRepository(_settingsManager, ConfigRepository, AdministratorsInRolesRepository, RoleRepository);

        private static ITemplateLogRepository _templateLogRepository;
        public static ITemplateLogRepository TemplateLogRepository => _templateLogRepository ??= new TemplateLogRepository(_settingsManager, AdministratorRepository);

        private static ILogRepository _logRepository;
        public static ILogRepository LogRepository => _logRepository ??= new LogRepository(_settingsManager, ConfigRepository, AdministratorRepository, ErrorLogRepository);

        private static ITemplateRepository _templateRepository;
        public static ITemplateRepository TemplateRepository => _templateRepository ??= new TemplateRepository(_settingsManager, TemplateLogRepository);

        private static IChannelRepository _channelRepository;
        public static IChannelRepository ChannelRepository => _channelRepository ??= new ChannelRepository(_settingsManager, TableStyleRepository, TemplateRepository);

        private static ISiteRepository _siteRepository;
        public static ISiteRepository SiteRepository => _siteRepository ??= new SiteRepository(_settingsManager, ChannelRepository, AdministratorRepository, TemplateRepository, TableStyleRepository, ContentGroupRepository, ContentTagRepository);

        private static IContentRepository _contentRepository;
        public static IContentRepository ContentRepository => _contentRepository ??= new ContentRepository(_settingsManager, AdministratorRepository, ChannelRepository, SiteRepository);

        private static IDatabaseRepository _databaseRepository;
        public static IDatabaseRepository DatabaseRepository => _databaseRepository ??= new DatabaseRepository(AccessTokenRepository, AdministratorRepository, AdministratorsInRolesRepository, ChannelGroupRepository, ChannelRepository, ConfigRepository, ContentCheckRepository, ContentGroupRepository, ContentRepository, ContentTagRepository, DbCacheRepository, ErrorLogRepository, LibraryFileRepository, LibraryGroupRepository, LibraryImageRepository, LibraryTextRepository, LibraryVideoRepository, LogRepository, PermissionsInRolesRepository, PluginConfigRepository, PluginRepository, RelatedFieldItemRepository, RelatedFieldRepository, RoleRepository, SiteLogRepository, SitePermissionsRepository, SiteRepository, SpecialRepository, TableStyleRepository, TemplateLogRepository, TemplateRepository, UserGroupRepository, UserLogRepository, UserMenuRepository, UserRepository);

        public static void Reset()
        {
            _accessTokenRepository = null;
            _administratorRepository = null;
            _administratorsInRolesRepository = null;
            _libraryGroupRepository = null;
            _libraryImageRepository = null;
            _libraryVideoRepository = null;
            _libraryFileRepository = null;
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
            LibraryVideoRepository,
            LibraryFileRepository,
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
