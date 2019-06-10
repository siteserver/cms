using System.Collections.Generic;
using SS.CMS.Core.Repositories;
using SS.CMS.Core.Settings;

namespace SS.CMS.Core.Common
{
    public static class DataProvider
    {
        private static AdministratorDao _administratorDao;
        public static AdministratorDao AdministratorDao => _administratorDao ?? (_administratorDao = new AdministratorDao(AppContext.Db));

        private static AdministratorsInRolesDao _administratorsInRolesDao;
        public static AdministratorsInRolesDao AdministratorsInRolesDao => _administratorsInRolesDao ?? (_administratorsInRolesDao = new AdministratorsInRolesDao(AppContext.Db));

        private static AreaDao _areaDao;
        public static AreaDao AreaDao => _areaDao ?? (_areaDao = new AreaDao(AppContext.Db));

        private static ChannelDao _channelDao;
        public static ChannelDao ChannelDao => _channelDao ?? (_channelDao = new ChannelDao(AppContext.Db));

        private static ChannelGroupDao _channelGroupDao;
        public static ChannelGroupDao ChannelGroupDao => _channelGroupDao ?? (_channelGroupDao = new ChannelGroupDao(AppContext.Db));

        private static ConfigDao _configDao;
        public static ConfigDao ConfigDao => _configDao ?? (_configDao = new ConfigDao(AppContext.Db));

        private static ContentCheckDao _contentCheckDao;
        public static ContentCheckDao ContentCheckDao => _contentCheckDao ?? (_contentCheckDao = new ContentCheckDao(AppContext.Db));

        private static ContentGroupDao _contentGroupDao;
        public static ContentGroupDao ContentGroupDao => _contentGroupDao ?? (_contentGroupDao = new ContentGroupDao(AppContext.Db));

        private static DbCacheDao _dbCacheDao;
        public static DbCacheDao DbCacheDao => _dbCacheDao ?? (_dbCacheDao = new DbCacheDao(AppContext.Db));

        private static DepartmentDao _departmentDao;
        public static DepartmentDao DepartmentDao => _departmentDao ?? (_departmentDao = new DepartmentDao(AppContext.Db));

        private static ErrorLogDao _errorLogDao;
        public static ErrorLogDao ErrorLogDao => _errorLogDao ?? (_errorLogDao = new ErrorLogDao(AppContext.Db));

        private static KeywordDao _keywordDao;
        public static KeywordDao KeywordDao => _keywordDao ?? (_keywordDao = new KeywordDao(AppContext.Db));

        private static LogDao _logDao;
        public static LogDao LogDao => _logDao ?? (_logDao = new LogDao(AppContext.Db));

        private static PermissionsInRolesDao _permissionsInRolesDao;
        public static PermissionsInRolesDao PermissionsInRolesDao => _permissionsInRolesDao ?? (_permissionsInRolesDao = new PermissionsInRolesDao(AppContext.Db));

        private static PluginConfigDao _pluginConfigDao;
        public static PluginConfigDao PluginConfigDao => _pluginConfigDao ?? (_pluginConfigDao = new PluginConfigDao(AppContext.Db));

        private static PluginDao _pluginDao;
        public static PluginDao PluginDao => _pluginDao ?? (_pluginDao = new PluginDao(AppContext.Db));

        private static RelatedFieldDao _relatedFieldDao;
        public static RelatedFieldDao RelatedFieldDao => _relatedFieldDao ?? (_relatedFieldDao = new RelatedFieldDao(AppContext.Db));

        private static RelatedFieldItemDao _relatedFieldItemDao;
        public static RelatedFieldItemDao RelatedFieldItemDao => _relatedFieldItemDao ?? (_relatedFieldItemDao = new RelatedFieldItemDao(AppContext.Db));

        private static RoleDao _roleDao;
        public static RoleDao RoleDao => _roleDao ?? (_roleDao = new RoleDao(AppContext.Db));

        private static SiteDao _siteDao;
        public static SiteDao SiteDao => _siteDao ?? (_siteDao = new SiteDao(AppContext.Db));

        private static SiteLogDao _siteLogDao;
        public static SiteLogDao SiteLogDao => _siteLogDao ?? (_siteLogDao = new SiteLogDao(AppContext.Db));

        private static SitePermissionsDao _sitePermissionsDao;
        public static SitePermissionsDao SitePermissionsDao => _sitePermissionsDao ?? (_sitePermissionsDao = new SitePermissionsDao(AppContext.Db));

        private static SpecialDao _specialDao;
        public static SpecialDao SpecialDao => _specialDao ?? (_specialDao = new SpecialDao(AppContext.Db));

        private static TableStyleDao _tableStyleDao;
        public static TableStyleDao TableStyleDao => _tableStyleDao ?? (_tableStyleDao = new TableStyleDao(AppContext.Db));

        private static TableStyleItemDao _tableStyleItemDao;
        public static TableStyleItemDao TableStyleItemDao => _tableStyleItemDao ?? (_tableStyleItemDao = new TableStyleItemDao(AppContext.Db));

        private static TagDao _tagDao;
        public static TagDao TagDao => _tagDao ?? (_tagDao = new TagDao());

        private static TemplateDao _templateDao;
        public static TemplateDao TemplateDao => _templateDao ?? (_templateDao = new TemplateDao());

        private static TemplateLogDao _templateLogDao;
        public static TemplateLogDao TemplateLogDao => _templateLogDao ?? (_templateLogDao = new TemplateLogDao());

        private static UserDao _userDao;
        public static UserDao UserDao => _userDao ?? (_userDao = new UserDao());

        private static UserGroupDao _userGroupDao;
        public static UserGroupDao UserGroupDao => _userGroupDao ?? (_userGroupDao = new UserGroupDao());

        private static UserLogDao _userLogDao;
        public static UserLogDao UserLogDao => _userLogDao ?? (_userLogDao = new UserLogDao());

        private static UserMenuDao _userMenuDao;
        public static UserMenuDao UserMenuDao => _userMenuDao ?? (_userMenuDao = new UserMenuDao());

        public static void Reset()
        {
            _administratorDao = null;
            _administratorsInRolesDao = null;
            _areaDao = null;
            _channelDao = null;
            _channelGroupDao = null;
            _configDao = null;
            _contentCheckDao = null;
            _contentGroupDao = null;
            _dbCacheDao = null;
            _departmentDao = null;
            _errorLogDao = null;
            _keywordDao = null;
            _logDao = null;
            _permissionsInRolesDao = null;
            _pluginConfigDao = null;
            _pluginDao = null;
            _relatedFieldDao = null;
            _relatedFieldItemDao = null;
            _roleDao = null;
            _siteDao = null;
            _siteLogDao = null;
            _sitePermissionsDao = null;
            _specialDao = null;
            _tableStyleDao = null;
            _tableStyleItemDao = null;
            _tagDao = null;
            _templateDao = null;
            _templateLogDao = null;
            _userDao = null;
            _userGroupDao = null;
            _userLogDao = null;
            _userMenuDao = null;
        }

        public static List<IDatabaseDao> AllProviders => new List<IDatabaseDao>
        {
            AdministratorDao,
            AdministratorsInRolesDao,
            AreaDao,
            ChannelDao,
            ChannelGroupDao,
            ConfigDao,
            ContentCheckDao,
            ContentGroupDao,
            DbCacheDao,
            DepartmentDao,
            ErrorLogDao,
            KeywordDao,
            LogDao,
            PermissionsInRolesDao,
            PluginConfigDao,
            PluginDao,
            RelatedFieldDao,
            RelatedFieldItemDao,
            RoleDao,
            SiteDao,
            SiteLogDao,
            SitePermissionsDao,
            SpecialDao,
            TableStyleDao,
            TableStyleItemDao,
            TagDao,
            TemplateDao,
            TemplateLogDao,
            UserDao,
            UserGroupDao,
            UserLogDao,
            UserMenuDao
        };
    }
}
