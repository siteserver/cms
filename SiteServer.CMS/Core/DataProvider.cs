using System.Collections.Generic;
using BaiRong.Core.Data;
using SiteServer.CMS.Provider;
using ContentDao = SiteServer.CMS.Provider.ContentDao;
using LogDao = SiteServer.CMS.Provider.LogDao;

namespace SiteServer.CMS.Core
{
    public class DataProvider
    {
        private static AdAreaDao _adAreaDao;
        public static AdAreaDao AdAreaDao => _adAreaDao ?? (_adAreaDao = new AdAreaDao());

        private static AdMaterialDao _adMaterialDao;
        public static AdMaterialDao AdMaterialDao => _adMaterialDao ?? (_adMaterialDao = new AdMaterialDao());

        private static AdvDao _advDao;
        public static AdvDao AdvDao => _advDao ?? (_advDao = new AdvDao());

        private static AdvertisementDao _advertisementDao;
        public static AdvertisementDao AdvertisementDao => _advertisementDao ?? (_advertisementDao = new AdvertisementDao());

        private static CommentDao _commentDao;
        public static CommentDao CommentDao => _commentDao ?? (_commentDao = new CommentDao());

        private static ContentDao _contentDao;
        public static ContentDao ContentDao => _contentDao ?? (_contentDao = new ContentDao());

        private static ContentGroupDao _contentGroupDao;
        public static ContentGroupDao ContentGroupDao => _contentGroupDao ?? (_contentGroupDao = new ContentGroupDao());

        private static CreateTaskDao _createTaskDao;
        public static CreateTaskDao CreateTaskDao => _createTaskDao ?? (_createTaskDao = new CreateTaskDao());

        private static CreateTaskLogDao _createTaskLogDao;
        public static CreateTaskLogDao CreateTaskLogDao => _createTaskLogDao ?? (_createTaskLogDao = new CreateTaskLogDao());

        private static GatherDatabaseRuleDao _gatherDatabaseRuleDao;
        public static GatherDatabaseRuleDao GatherDatabaseRuleDao => _gatherDatabaseRuleDao ?? (_gatherDatabaseRuleDao = new GatherDatabaseRuleDao());

        private static GatherFileRuleDao _gatherFileRuleDao;
        public static GatherFileRuleDao GatherFileRuleDao => _gatherFileRuleDao ?? (_gatherFileRuleDao = new GatherFileRuleDao());

        private static GatherRuleDao _gatherRuleDao;
        public static GatherRuleDao GatherRuleDao => _gatherRuleDao ?? (_gatherRuleDao = new GatherRuleDao());

        private static InnerLinkDao _innerLinkDao;
        public static InnerLinkDao InnerLinkDao => _innerLinkDao ?? (_innerLinkDao = new InnerLinkDao());

        private static KeywordDao _keywordDao;
        public static KeywordDao KeywordDao => _keywordDao ?? (_keywordDao = new KeywordDao());

        private static LogDao _logDao;
        public static LogDao LogDao => _logDao ?? (_logDao = new LogDao());

        private static MenuDisplayDao _menuDisplayDao;
        public static MenuDisplayDao MenuDisplayDao => _menuDisplayDao ?? (_menuDisplayDao = new MenuDisplayDao());

        private static NodeDao _nodeDao;
        public static NodeDao NodeDao => _nodeDao ?? (_nodeDao = new NodeDao());

        private static NodeGroupDao _nodeGroupDao;
        public static NodeGroupDao NodeGroupDao => _nodeGroupDao ?? (_nodeGroupDao = new NodeGroupDao());

        private static PhotoDao _photoDao;
        public static PhotoDao PhotoDao => _photoDao ?? (_photoDao = new PhotoDao());

        private static PluginConfigDao _pluginConfigDao;
        public static PluginConfigDao PluginConfigDao => _pluginConfigDao ?? (_pluginConfigDao = new PluginConfigDao());

        private static PublishmentSystemDao _publishmentSystemDao;
        public static PublishmentSystemDao PublishmentSystemDao => _publishmentSystemDao ?? (_publishmentSystemDao = new PublishmentSystemDao());

        private static RelatedFieldDao _relatedFieldDao;
        public static RelatedFieldDao RelatedFieldDao => _relatedFieldDao ?? (_relatedFieldDao = new RelatedFieldDao());

        private static RelatedFieldItemDao _relatedFieldItemDao;
        public static RelatedFieldItemDao RelatedFieldItemDao => _relatedFieldItemDao ?? (_relatedFieldItemDao = new RelatedFieldItemDao());

        private static SeoMetaDao _seoMetaDao;
        public static SeoMetaDao SeoMetaDao => _seoMetaDao ?? (_seoMetaDao = new SeoMetaDao());

        private static SeoMetasInNodesDao _seoMetasInNodesDao;
        public static SeoMetasInNodesDao SeoMetasInNodesDao => _seoMetasInNodesDao ?? (_seoMetasInNodesDao = new SeoMetasInNodesDao());

        private static StarDao _starDao;
        public static StarDao StarDao => _starDao ?? (_starDao = new StarDao());

        private static StarSettingDao _starSettingDao;
        public static StarSettingDao StarSettingDao => _starSettingDao ?? (_starSettingDao = new StarSettingDao());

        private static SystemPermissionsDao _systemPermissionsDao;
        public static SystemPermissionsDao SystemPermissionsDao => _systemPermissionsDao ?? (_systemPermissionsDao = new SystemPermissionsDao());

        private static TagStyleDao _tagStyleDao;
        public static TagStyleDao TagStyleDao => _tagStyleDao ?? (_tagStyleDao = new TagStyleDao());

        private static TaskDao _taskDao;
        public static TaskDao TaskDao => _taskDao ?? (_taskDao = new TaskDao());

        private static TaskLogDao _taskLogDao;
        public static TaskLogDao TaskLogDao => _taskLogDao ?? (_taskLogDao = new TaskLogDao());

        private static TemplateDao _templateDao;
        public static TemplateDao TemplateDao => _templateDao ?? (_templateDao = new TemplateDao());

        private static TemplateLogDao _templateLogDao;
        public static TemplateLogDao TemplateLogDao => _templateLogDao ?? (_templateLogDao = new TemplateLogDao());

        private static TemplateMatchDao _templateMatchDao;
        public static TemplateMatchDao TemplateMatchDao => _templateMatchDao ?? (_templateMatchDao = new TemplateMatchDao());

        private static TrackingDao _trackingDao;
        public static TrackingDao TrackingDao => _trackingDao ?? (_trackingDao = new TrackingDao());

        public static List<DataProviderBase> AllProviders => new List<DataProviderBase>
        {
            AdAreaDao,
            AdMaterialDao,
            AdvDao,
            AdvertisementDao,
            CommentDao,
            ContentDao,
            ContentGroupDao,
            CreateTaskDao,
            CreateTaskLogDao,
            GatherDatabaseRuleDao,
            GatherFileRuleDao,
            GatherRuleDao,
            InnerLinkDao,
            KeywordDao,
            LogDao,
            MenuDisplayDao,
            NodeDao,
            NodeGroupDao,
            PhotoDao,
            PluginConfigDao,
            PublishmentSystemDao,
            RelatedFieldDao,
            RelatedFieldItemDao,
            SeoMetaDao,
            SeoMetasInNodesDao,
            StarDao,
            StarSettingDao,
            SystemPermissionsDao,
            TagStyleDao,
            TaskDao,
            TaskLogDao,
            TemplateDao,
            TemplateLogDao,
            TemplateMatchDao,
            TrackingDao
        };
    }
}
