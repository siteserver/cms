using System;
using System.Threading.Tasks;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class WxManager : IWxManager
    {
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

        public WxManager(ISettingsManager settingsManager, IPathManager pathManager, ITaskManager taskManager, IWxAccountRepository wxAccountRepository,
            IWxMenuRepository wxMenuRepository, IWxChatRepository wxChatRepository, IWxReplyRuleRepository wxReplyRuleRepository,
            IWxReplyKeywordRepository wxReplyKeywordRepository, IWxReplyMessageRepository wxReplyMessageRepository,
            IMaterialMessageRepository materialMessageRepository, IMaterialArticleRepository materialArticleRepository, IMaterialImageRepository materialImageRepository,
            IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository)
        {
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
        }

        public string GetNonceStr()
        {
            return JSSDKHelper.GetNoncestr();
        }

        public string GetTimestamp()
        {
            return JSSDKHelper.GetTimestamp();
        }

        public async Task<(bool Success, string AccessToken, string ErrorMessage)> GetAccessTokenAsync(int siteId)
        {
            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);
            if (string.IsNullOrEmpty(account.MpAppId) || string.IsNullOrEmpty(account.MpAppSecret))
            {
                return (false, null, "微信公众号AppId及AppSecret未设置，请到平台账号配置中设置");
            }

            return await GetAccessTokenAsync(account.MpAppId, account.MpAppSecret);
        }

        public async Task<(bool Success, string AccessToken, string ErrorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret)
        {
            var success = false;
            var errorMessage = string.Empty;
            string token = null;

            try
            {
                token = await AccessTokenContainer.TryGetAccessTokenAsync(mpAppId, mpAppSecret);
                success = true;
            }
            catch (ErrorJsonResultException ex)
            {
                if (ex.JsonResult.errcode == ReturnCode.调用接口的IP地址不在白名单中)
                {
                    var startIndex = ex.JsonResult.errmsg.IndexOf("invalid ip ", StringComparison.Ordinal) + 11;
                    var endIndex = ex.JsonResult.errmsg.IndexOf(" ipv6", StringComparison.Ordinal);
                    var ip = ex.JsonResult.errmsg.Substring(startIndex, endIndex - startIndex);
                    errorMessage = $"调用接口的IP地址不在白名单中，请进入微信公众平台，将本服务器的IP地址 {ip} 添加至白名单";
                }
                else
                {
                    errorMessage = $"API 调用发生错误：{ex.JsonResult.errmsg}";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"执行过程发生错误：{ex.Message}";
            }

            return (success, token, errorMessage);
        }

        public async Task<(bool Success, string Ticket, string ErrorMessage)> GetJsApiTicketAsync(string mpAppId, string mpAppSecret)
        {
            var success = false;
            var errorMessage = string.Empty;
            string ticket = null;

            try
            {
                ticket = await JsApiTicketContainer.TryGetJsApiTicketAsync(mpAppId, mpAppSecret);
                success = true;
            }
            catch (ErrorJsonResultException ex)
            {
                if (ex.JsonResult.errcode == ReturnCode.调用接口的IP地址不在白名单中)
                {
                    var startIndex = ex.JsonResult.errmsg.IndexOf("invalid ip ", StringComparison.Ordinal) + 11;
                    var endIndex = ex.JsonResult.errmsg.IndexOf(" ipv6", StringComparison.Ordinal);
                    var ip = ex.JsonResult.errmsg.Substring(startIndex, endIndex - startIndex);
                    errorMessage = $"调用接口的IP地址不在白名单中，请进入微信公众平台，将本服务器的IP地址 {ip} 添加至白名单";
                }
                else
                {
                    errorMessage = $"API 调用发生错误：{ex.JsonResult.errmsg}";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"执行过程发生错误：{ex.Message}";
            }

            return (success, ticket, errorMessage);
        }

        public string GetJsApiSignature(string ticket, string nonceStr, string timestamp, string url)
        {
            return JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, url);
        }

        private GroupMessageType GetGroupMessageType(MaterialType materialType)
        {
            if (materialType == MaterialType.Message) return GroupMessageType.mpnews;
            if (materialType == MaterialType.Text) return GroupMessageType.text;
            if (materialType == MaterialType.Image) return GroupMessageType.image;
            if (materialType == MaterialType.Audio) return GroupMessageType.voice;
            if (materialType == MaterialType.Video) return GroupMessageType.video;
            return GroupMessageType.mpnews;
        }

        private string SaveImages(string content)
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

                WebClientUtils.Download(originalImageSrc, filePath);

                var imageUrl = PageUtils.Combine(virtualDirectoryPath, materialFileName);

                content = content.Replace(" data-src=", "src=");
                content = content.Replace(originalImageSrc, imageUrl);
            }
            return content;
        }
    }
}
