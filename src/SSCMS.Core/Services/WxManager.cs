using System;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

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
            public string towxname { get; set; }
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

        public class StableTokenBody
        {
            public string grant_type {get;set;}
            public string appid {get;set;}
            public string secret {get;set;}
            public bool force_refresh {get;set;}
        }

        private async Task<string> SaveImagesAsync(string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc)) continue;

                var extName = "png";
                if (StringUtils.Contains(originalImageSrc, "wx_fmt="))
                {
                    extName = originalImageSrc.Substring(originalImageSrc.LastIndexOf("=", StringComparison.Ordinal) + 1);
                }

                var materialFileName = PathUtils.GetMaterialFileNameByExtName(extName);
                var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image);

                var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, materialFileName);

                await HttpClientUtils.DownloadAsync(originalImageSrc, filePath);

                var imageUrl = PageUtils.Combine(virtualDirectoryPath, materialFileName);

                content = content.Replace(" data-src=", "src=");
                content = content.Replace(originalImageSrc, imageUrl);
            }
            return content;
        }

        public async Task<bool> IsEnabledAsync(Site site)
        {
            if (!StringUtils.EqualsIgnoreCase(site.SiteType, Configuration.Types.SiteTypes.Wx))
            {
                return false;
            }
            var account = await GetAccountAsync(site.Id);
            return account != null && account.IsEnabled && !string.IsNullOrEmpty(account.MpAppId) && !string.IsNullOrEmpty(account.MpAppSecret);
        }

        private async Task<(bool success, string errorMessage)> GetErrorMessageAsync(string result)
        {
            bool success = true;
            string errorMessage = string.Empty;

            if (StringUtils.Contains(result, "errcode"))
            {
                success = false;
                var jsonError = TranslateUtils.JsonDeserialize<JsonResult>(result);

                if (jsonError.errcode == 40013)
                {
                    errorMessage = "不合法的 AppID ，请检查 AppID 的正确性，避免异常字符，注意大小写。";
                }
                else if (jsonError.errcode == 40125)
                {
                    errorMessage = "无效的appsecret，请检查appsecret的正确性。";
                }
                else if (jsonError.errcode == 40164)
                {
                    var startIndex = jsonError.errmsg.IndexOf("invalid ip ", StringComparison.Ordinal) + 11;
                    var endIndex = jsonError.errmsg.IndexOf(" ipv6", StringComparison.Ordinal);
                    var ip = jsonError.errmsg.Substring(startIndex, endIndex - startIndex);
                    errorMessage = $"调用接口的IP地址不在白名单中，请进入微信公众平台，将本服务器的IP地址 {ip} 添加至白名单，如果已配置，请等待 10 分钟左右再试。";
                }
                else if (jsonError.errcode == 45009)
                {
                    errorMessage = "调用超过天级别频率限制。可调用clear_quota接口恢复调用额度。";
                }
                else if (jsonError.errcode == 45011)
                {
                    errorMessage = "API 调用太频繁，请稍候再试。";
                }
                else
                {
                    errorMessage = $"API 调用发生错误：{jsonError.errmsg}";
                }

                await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.GetAccessTokenAsync");
            }

            return (success, errorMessage);
        }
    }
}
