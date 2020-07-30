using System;
using System.Threading.Tasks;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Containers;
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
        private readonly IWxAccountRepository _openAccountRepository;
        private readonly IWxMenuRepository _openMenuRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public WxManager(ISettingsManager settingsManager, IWxAccountRepository openAccountRepository, IWxMenuRepository openMenuRepository, IMaterialMessageRepository materialMessageRepository, IMaterialImageRepository materialImageRepository, IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _settingsManager = settingsManager;
            _openAccountRepository = openAccountRepository;
            _openMenuRepository = openMenuRepository;
            _materialMessageRepository = materialMessageRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        public async Task<(bool, string, string)> GetAccessTokenAsync(int siteId)
        {
            var account = await _openAccountRepository.GetBySiteIdAsync(siteId);
            if (string.IsNullOrEmpty(account.MpAppId) || string.IsNullOrEmpty(account.MpAppSecret))
            {
                return (false, null, "微信公众号AppId及AppSecret未设置，请到平台账号配置中设置");
            }

            return await GetAccessTokenAsync(account.MpAppId, account.MpAppSecret);
        }

        public async Task<(bool, string, string)> GetAccessTokenAsync(string mpAppId, string mpAppSecret)
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
