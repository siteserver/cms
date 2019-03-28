using System.Collections.Generic;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Database.Repositories;
using SiteServer.CMS.Database.Repositories.Contents;

namespace SiteServer.CMS.Database.Core
{
    public static class DataProvider
    {
        private static AccessTokenRepository _accessToken;
        public static AccessTokenRepository AccessToken => _accessToken ?? (_accessToken = new AccessTokenRepository());

        private static AdministratorRepository _administrator;
        public static AdministratorRepository Administrator => _administrator ?? (_administrator = new AdministratorRepository());

        private static AdministratorsInRolesRepository _administratorsInRoles;
        public static AdministratorsInRolesRepository AdministratorsInRoles => _administratorsInRoles ?? (_administratorsInRoles = new AdministratorsInRolesRepository());

        private static AreaRepository _area;
        public static AreaRepository Area => _area ?? (_area = new AreaRepository());

        private static ChannelRepository _channel;
        public static ChannelRepository Channel => _channel ?? (_channel = new ChannelRepository());

        private static ChannelGroupRepository _channelGroup;
        public static ChannelGroupRepository ChannelGroup => _channelGroup ?? (_channelGroup = new ChannelGroupRepository());

        private static ConfigRepository _config;
        public static ConfigRepository Config => _config ?? (_config = new ConfigRepository());

        private static ContentCheckRepository _contentCheck;
        public static ContentCheckRepository ContentCheck => _contentCheck ?? (_contentCheck = new ContentCheckRepository());

        private static ContentGroupRepository _contentGroup;
        public static ContentGroupRepository ContentGroup => _contentGroup ?? (_contentGroup = new ContentGroupRepository());

        private static ContentTagRepository _contentTag;
        public static ContentTagRepository ContentTag => _contentTag ?? (_contentTag = new ContentTagRepository());

        private static DbCacheRepository _dbCache;
        public static DbCacheRepository DbCache => _dbCache ?? (_dbCache = new DbCacheRepository());

        private static DepartmentRepository _department;
        public static DepartmentRepository Department => _department ?? (_department = new DepartmentRepository());

        private static ErrorLogRepository _errorLog;
        public static ErrorLogRepository ErrorLog => _errorLog ?? (_errorLog = new ErrorLogRepository());

        private static KeywordRepository _keyword;
        public static KeywordRepository Keyword => _keyword ?? (_keyword = new KeywordRepository());

        private static LogRepository _log;
        public static LogRepository Log => _log ?? (_log = new LogRepository());

        private static PermissionsInRolesRepository _permissionsInRoles;
        public static PermissionsInRolesRepository PermissionsInRoles => _permissionsInRoles ?? (_permissionsInRoles = new PermissionsInRolesRepository());

        private static PluginConfigRepository _pluginConfig;
        public static PluginConfigRepository PluginConfig => _pluginConfig ?? (_pluginConfig = new PluginConfigRepository());

        private static PluginRepository _plugin;
        public static PluginRepository Plugin => _plugin ?? (_plugin = new PluginRepository());

        private static RelatedFieldRepository _relatedField;
        public static RelatedFieldRepository RelatedField => _relatedField ?? (_relatedField = new RelatedFieldRepository());

        private static RelatedFieldItemRepository _relatedFieldItem;
        public static RelatedFieldItemRepository RelatedFieldItem => _relatedFieldItem ?? (_relatedFieldItem = new RelatedFieldItemRepository());

        private static RoleRepository _role;
        public static RoleRepository Role => _role ?? (_role = new RoleRepository());

        private static SiteRepository _site;
        public static SiteRepository Site => _site ?? (_site = new SiteRepository());

        private static SiteLogRepository _siteLog;
        public static SiteLogRepository SiteLog => _siteLog ?? (_siteLog = new SiteLogRepository());

        private static SitePermissionsRepository _sitePermissions;
        public static SitePermissionsRepository SitePermissions => _sitePermissions ?? (_sitePermissions = new SitePermissionsRepository());

        private static SpecialRepository _special;
        public static SpecialRepository Special => _special ?? (_special = new SpecialRepository());

        private static TableStyleRepository _tableStyle;
        public static TableStyleRepository TableStyle => _tableStyle ?? (_tableStyle = new TableStyleRepository());

        private static TableStyleItemRepository _tableStyleItem;
        public static TableStyleItemRepository TableStyleItem => _tableStyleItem ?? (_tableStyleItem = new TableStyleItemRepository());

        private static TagRepository _tag;
        public static TagRepository Tag => _tag ?? (_tag = new TagRepository());

        private static TemplateRepository _template;
        public static TemplateRepository Template => _template ?? (_template = new TemplateRepository());

        private static TemplateLogRepository _templateLog;
        public static TemplateLogRepository TemplateLog => _templateLog ?? (_templateLog = new TemplateLogRepository());

        private static UserRepository _user;
        public static UserRepository User => _user ?? (_user = new UserRepository());

        private static UserGroupRepository _userGroup;
        public static UserGroupRepository UserGroup => _userGroup ?? (_userGroup = new UserGroupRepository());

        private static UserLogRepository _userLog;
        public static UserLogRepository UserLog => _userLog ?? (_userLog = new UserLogRepository());

        private static UserMenuRepository _userMenu;
        public static UserMenuRepository UserMenu => _userMenu ?? (_userMenu = new UserMenuRepository());

        private static ContentRepository _contentRepository;
        public static ContentRepository ContentRepository => _contentRepository ?? (_contentRepository = new ContentRepository());

        public static void Reset()
        {
            DatabaseApi.Instance = null;

            _contentRepository = null;

            _accessToken = null;
            _administrator = null;
            _administratorsInRoles = null;
            _area = null;
            _channel = null;
            _channelGroup = null;
            _config = null;
            _contentCheck = null;
            _contentGroup = null;
            _contentTag = null;
            _dbCache = null;
            _department = null;
            _errorLog = null;
            _keyword = null;
            _log = null;
            _permissionsInRoles = null;
            _pluginConfig = null;
            _plugin = null;
            _relatedField = null;
            _relatedFieldItem = null;
            _role = null;
            _site = null;
            _siteLog = null;
            _sitePermissions = null;
            _special = null;
            _tableStyle = null;
            _tableStyleItem = null;
            _tag = null;
            _template = null;
            _templateLog = null;
            _user = null;
            _userGroup = null;
            _userLog = null;
            _userMenu = null;
        }

        public static IEnumerable<IRepository> AllRepositories => new List<IRepository>
        {
            AccessToken,
            Administrator,
            AdministratorsInRoles,
            Area,
            Channel,
            ChannelGroup,
            Config,
            ContentCheck,
            ContentGroup,
            ContentTag,
            DbCache,
            Department,
            ErrorLog,
            Keyword,
            Log,
            PermissionsInRoles,
            PluginConfig,
            Plugin,
            RelatedField,
            RelatedFieldItem,
            Role,
            Site,
            SiteLog,
            SitePermissions,
            Special,
            TableStyle,
            TableStyleItem,
            Tag,
            Template,
            TemplateLog,
            User,
            UserGroup,
            UserLog,
            UserMenu
        };
    }
}
