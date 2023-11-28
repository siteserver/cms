using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public partial class WxManager : IWxManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ITaskManager _taskManager;
        private readonly IWxAccountRepository _wxAccountRepository;
        private readonly IWxMenuRepository _wxMenuRepository;
        private readonly IWxChatRepository _wxChatRepository;
        private readonly IWxReplyRuleRepository _wxReplyRuleRepository;
        private readonly IWxReplyKeywordRepository _wxReplyKeywordRepository;
        private readonly IWxReplyMessageRepository _wxReplyMessageRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;
        private readonly IMaterialArticleRepository _materialArticleRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public WxManager(ICacheManager cacheManager, ISettingsManager settingsManager, IPathManager pathManager, ITaskManager taskManager, IWxAccountRepository wxAccountRepository,
            IWxMenuRepository wxMenuRepository, IWxChatRepository wxChatRepository, IWxReplyRuleRepository wxReplyRuleRepository,
            IWxReplyKeywordRepository wxReplyKeywordRepository, IWxReplyMessageRepository wxReplyMessageRepository,
            IMaterialMessageRepository materialMessageRepository, IMaterialArticleRepository materialArticleRepository, IMaterialImageRepository materialImageRepository,
            IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository, IErrorLogRepository errorLogRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _taskManager = taskManager;
            _wxAccountRepository = wxAccountRepository;
            _wxMenuRepository = wxMenuRepository;
            _wxChatRepository = wxChatRepository;
            _wxReplyRuleRepository = wxReplyRuleRepository;
            _wxReplyKeywordRepository = wxReplyKeywordRepository;
            _wxReplyMessageRepository = wxReplyMessageRepository;
            _materialMessageRepository = materialMessageRepository;
            _materialArticleRepository = materialArticleRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
            _errorLogRepository = errorLogRepository;
        }

        public class AccessToken
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public class JsonResult
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }
        }

        public class JsonMediaId
        {
            public string media_id { get; set; }
        }

        public class FreePublishSubmitResult
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }
            public string publish_id { get; set; }
        }

        public class JsonPreviewRequest
        {
            public string touser { get; set; }
            public JsonMediaId mpnews { get; set; }
            public string msgtype { get; set; }
        }

        public class DraftArticle
        {
            /// <summary>
            /// 图文消息的标题
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// 图文消息的作者
            /// </summary>
            public string author { get; set; }

            /// <summary>
            /// 图文消息的描述
            /// </summary>
            public string digest { get; set; }

            /// <summary>
            /// 图文消息页面的内容，支持HTML标签
            /// </summary>
            public string content { get; set; }

            /// <summary>
            /// 在图文消息页面点击“阅读原文”后的页面
            /// </summary>
            public string content_source_url { get; set; }

            /// <summary>
            /// 图文消息缩略图的media_id，可以在基础支持上传多媒体文件接口中获得
            /// </summary>
            public string thumb_media_id { get; set; }

            /// <summary>
            /// 是否打开评论，0不打开，1打开
            /// </summary>
            public int need_open_comment { get; set; }
            /// <summary>
            /// 是否粉丝才可评论，0所有人可评论，1粉丝才可评论
            /// </summary>
            public int only_fans_can_comment { get; set; }
        }
    }
}
