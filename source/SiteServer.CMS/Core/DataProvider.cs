using SiteServer.CMS.Provider;
using SiteServer.CMS.Wcm.Provider;

namespace SiteServer.CMS.Core
{
    public class DataProvider
    {
        private static NodeGroupDao _nodeGroupDao;
        public static NodeGroupDao NodeGroupDao => _nodeGroupDao ?? (_nodeGroupDao = new NodeGroupDao());

        private static ContentGroupDao _contentGroupDao;
        public static ContentGroupDao ContentGroupDao => _contentGroupDao ?? (_contentGroupDao = new ContentGroupDao());

        private static NodeDao _nodeDao;
        public static NodeDao NodeDao => _nodeDao ?? (_nodeDao = new NodeDao());

        private static BackgroundContentDao _backgroundContentDao;
        public static BackgroundContentDao BackgroundContentDao => _backgroundContentDao ?? (_backgroundContentDao = new BackgroundContentDao());

        private static GovPublicContentDao _govPublicContentDao;
        public static GovPublicContentDao GovPublicContentDao => _govPublicContentDao ?? (_govPublicContentDao = new GovPublicContentDao());

        private static GovPublicCategoryClassDao _govPublicCategoryClassDao;
        public static GovPublicCategoryClassDao GovPublicCategoryClassDao => _govPublicCategoryClassDao ?? (_govPublicCategoryClassDao = new GovPublicCategoryClassDao());

        private static GovPublicCategoryDao _govPublicCategoryDao;
        public static GovPublicCategoryDao GovPublicCategoryDao => _govPublicCategoryDao ?? (_govPublicCategoryDao = new GovPublicCategoryDao());

        private static GovPublicChannelDao _govPublicChannelDao;
        public static GovPublicChannelDao GovPublicChannelDao => _govPublicChannelDao ?? (_govPublicChannelDao = new GovPublicChannelDao());

        private static GovPublicIdentifierRuleDao _govPublicIdentifierRuleDao;
        public static GovPublicIdentifierRuleDao GovPublicIdentifierRuleDao => _govPublicIdentifierRuleDao ?? (_govPublicIdentifierRuleDao = new GovPublicIdentifierRuleDao());

        private static GovPublicIdentifierSeqDao _govPublicIdentifierSeqDao;
        public static GovPublicIdentifierSeqDao GovPublicIdentifierSeqDao => _govPublicIdentifierSeqDao ?? (_govPublicIdentifierSeqDao = new GovPublicIdentifierSeqDao());

        private static GovPublicApplyDao _govPublicApplyDao;
        public static GovPublicApplyDao GovPublicApplyDao => _govPublicApplyDao ?? (_govPublicApplyDao = new GovPublicApplyDao());

        private static GovPublicApplyLogDao _govPublicApplyLogDao;
        public static GovPublicApplyLogDao GovPublicApplyLogDao => _govPublicApplyLogDao ?? (_govPublicApplyLogDao = new GovPublicApplyLogDao());

        private static GovPublicApplyRemarkDao _govPublicApplyRemarkDao;
        public static GovPublicApplyRemarkDao GovPublicApplyRemarkDao => _govPublicApplyRemarkDao ?? (_govPublicApplyRemarkDao = new GovPublicApplyRemarkDao());

        private static GovPublicApplyReplyDao _govPublicApplyReplyDao;
        public static GovPublicApplyReplyDao GovPublicApplyReplyDao => _govPublicApplyReplyDao ?? (_govPublicApplyReplyDao = new GovPublicApplyReplyDao());

        private static GovInteractChannelDao _govInteractChannelDao;
        public static GovInteractChannelDao GovInteractChannelDao => _govInteractChannelDao ?? (_govInteractChannelDao = new GovInteractChannelDao());

        private static GovInteractContentDao _govInteractContentDao;
        public static GovInteractContentDao GovInteractContentDao => _govInteractContentDao ?? (_govInteractContentDao = new GovInteractContentDao());

        private static GovInteractLogDao _govInteractApplyLogDao;
        public static GovInteractLogDao GovInteractLogDao => _govInteractApplyLogDao ?? (_govInteractApplyLogDao = new GovInteractLogDao());

        private static GovInteractRemarkDao _govInteractRemarkDao;
        public static GovInteractRemarkDao GovInteractRemarkDao => _govInteractRemarkDao ?? (_govInteractRemarkDao = new GovInteractRemarkDao());

        private static GovInteractReplyDao _govInteractApplyReplyDao;
        public static GovInteractReplyDao GovInteractReplyDao => _govInteractApplyReplyDao ?? (_govInteractApplyReplyDao = new GovInteractReplyDao());

        private static GovInteractTypeDao _govInteractTypeDao;
        public static GovInteractTypeDao GovInteractTypeDao => _govInteractTypeDao ?? (_govInteractTypeDao = new GovInteractTypeDao());

        private static GovInteractPermissionsDao _govInteractPermissionsDao;
        public static GovInteractPermissionsDao GovInteractPermissionsDao => _govInteractPermissionsDao ?? (_govInteractPermissionsDao = new GovInteractPermissionsDao());

        private static VoteContentDao _voteContentDao;
        public static VoteContentDao VoteContentDao => _voteContentDao ?? (_voteContentDao = new VoteContentDao());

        private static VoteOptionDao _voteOptionDao;
        public static VoteOptionDao VoteOptionDao => _voteOptionDao ?? (_voteOptionDao = new VoteOptionDao());

        private static VoteOperationDao _voteOperationDao;
        public static VoteOperationDao VoteOperationDao => _voteOperationDao ?? (_voteOperationDao = new VoteOperationDao());

        private static JobContentDao _jobContentDao;
        public static JobContentDao JobContentDao => _jobContentDao ?? (_jobContentDao = new JobContentDao());

        private static ContentDao _contentDao;
        public static ContentDao ContentDao => _contentDao ?? (_contentDao = new ContentDao());

        private static PublishmentSystemDao _publishmentSystemDao;
        public static PublishmentSystemDao PublishmentSystemDao => _publishmentSystemDao ?? (_publishmentSystemDao = new PublishmentSystemDao());

        private static TemplateDao _templateDao;
        public static TemplateDao TemplateDao => _templateDao ?? (_templateDao = new TemplateDao());

        private static TemplateLogDao _templateLogDao;
        public static TemplateLogDao TemplateLogDao => _templateLogDao ?? (_templateLogDao = new TemplateLogDao());

        private static MenuDisplayDao _menuDisplayDao;
        public static MenuDisplayDao MenuDisplayDao => _menuDisplayDao ?? (_menuDisplayDao = new MenuDisplayDao());

        private static TrackingDao _trackingDao;
        public static TrackingDao TrackingDao => _trackingDao ?? (_trackingDao = new TrackingDao());

        private static GatherRuleDao _gatherRuleDao;
        public static GatherRuleDao GatherRuleDao => _gatherRuleDao ?? (_gatherRuleDao = new GatherRuleDao());

        private static GatherDatabaseRuleDao _gatherDatabaseRuleDao;
        public static GatherDatabaseRuleDao GatherDatabaseRuleDao => _gatherDatabaseRuleDao ?? (_gatherDatabaseRuleDao = new GatherDatabaseRuleDao());

        private static GatherFileRuleDao _gatherFileRuleDao;
        public static GatherFileRuleDao GatherFileRuleDao => _gatherFileRuleDao ?? (_gatherFileRuleDao = new GatherFileRuleDao());

        private static AdvertisementDao _advertisementDao;
        public static AdvertisementDao AdvertisementDao => _advertisementDao ?? (_advertisementDao = new AdvertisementDao());

        private static AdAreaDao _adAreaDao;
        public static AdAreaDao AdAreaDao => _adAreaDao ?? (_adAreaDao = new AdAreaDao());

        private static AdvDao _advDao;
        public static AdvDao AdvDao => _advDao ?? (_advDao = new AdvDao());

        private static AdMaterialDao _adMaterialDao;
        public static AdMaterialDao AdMaterialDao => _adMaterialDao ?? (_adMaterialDao = new AdMaterialDao());

        private static SystemPermissionsDao _systemPermissionsDao;
        public static SystemPermissionsDao SystemPermissionsDao => _systemPermissionsDao ?? (_systemPermissionsDao = new SystemPermissionsDao());

        private static PermissionsDao _permissionsDao;
        public static PermissionsDao PermissionsDao => _permissionsDao ?? (_permissionsDao = new PermissionsDao());

        private static SeoMetaDao _seoMetaDao;
        public static SeoMetaDao SeoMetaDao => _seoMetaDao ?? (_seoMetaDao = new SeoMetaDao());

        private static StlTagDao _stlTagDao;
        public static StlTagDao StlTagDao => _stlTagDao ?? (_stlTagDao = new StlTagDao());

        private static InnerLinkDao _innerLinkDao;
        public static InnerLinkDao InnerLinkDao => _innerLinkDao ?? (_innerLinkDao = new InnerLinkDao());

        private static InputDao _inputDao;
        public static InputDao InputDao => _inputDao ?? (_inputDao = new InputDao());

        private static InputContentDao _inputContentDao;
        public static InputContentDao InputContentDao => _inputContentDao ?? (_inputContentDao = new InputContentDao());

        private static StarDao _starDao;
        public static StarDao StarDao => _starDao ?? (_starDao = new StarDao());

        private static StarSettingDao _starSettingDao;
        public static StarSettingDao StarSettingDao => _starSettingDao ?? (_starSettingDao = new StarSettingDao());

        private static TemplateMatchDao _templateMatchDao;
        public static TemplateMatchDao TemplateMatchDao => _templateMatchDao ?? (_templateMatchDao = new TemplateMatchDao());

        private static LogDao _logDao;
        public static LogDao LogDao => _logDao ?? (_logDao = new LogDao());

        private static TagStyleDao _tagStyleDao;
        public static TagStyleDao TagStyleDao => _tagStyleDao ?? (_tagStyleDao = new TagStyleDao());

        private static ResumeContentDao _resumeContentDao;
        public static ResumeContentDao ResumeContentDao => _resumeContentDao ?? (_resumeContentDao = new ResumeContentDao());

        private static PhotoDao _photoDao;
        public static PhotoDao PhotoDao => _photoDao ?? (_photoDao = new PhotoDao());

        private static RelatedFieldDao _relatedFieldDao;
        public static RelatedFieldDao RelatedFieldDao => _relatedFieldDao ?? (_relatedFieldDao = new RelatedFieldDao());

        private static RelatedFieldItemDao _relatedFieldItemDao;
        public static RelatedFieldItemDao RelatedFieldItemDao => _relatedFieldItemDao ?? (_relatedFieldItemDao = new RelatedFieldItemDao());

        private static CommentDao _commentDao;
        public static CommentDao CommentDao => _commentDao ?? (_commentDao = new CommentDao());

        private static TaskDao _taskDao;
        public static TaskDao TaskDao => _taskDao ?? (_taskDao = new TaskDao());

        private static TaskLogDao _taskLogDao;
        public static TaskLogDao TaskLogDao => _taskLogDao ?? (_taskLogDao = new TaskLogDao());

        private static CreateTaskDao _createTaskDao;
        public static CreateTaskDao CreateTaskDao => _createTaskDao ?? (_createTaskDao = new CreateTaskDao());

        private static CreateTaskLogDao _createTaskLogDao;
        public static CreateTaskLogDao CreateTaskLogDao => _createTaskLogDao ?? (_createTaskLogDao = new CreateTaskLogDao());

        private static KeywordDao _keywordDao;
        public static KeywordDao KeywordDao => _keywordDao ?? (_keywordDao = new KeywordDao());
    }
}
