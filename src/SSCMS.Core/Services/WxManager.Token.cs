using System;
using System.Threading.Tasks;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class WxManager : IWxManager
    {
        public string GetNonceStr()
        {
            return JSSDKHelper.GetNoncestr();
        }

        public string GetTimestamp()
        {
            return JSSDKHelper.GetTimestamp();
        }

        private static string GetCacheKey(string mpAppId)
        {
            return $"{nameof(WxManager)}.{mpAppId}";
        }

        public async Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(int siteId)
        {
            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);
            if (string.IsNullOrEmpty(account.MpAppId) || string.IsNullOrEmpty(account.MpAppSecret))
            {
                return (false, null, "微信公众号AppId及AppSecret未设置，请到平台账号配置中设置");
            }

            return await GetAccessTokenAsync(account.MpAppId, account.MpAppSecret);
        }

        public async Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret)
        {
            var success = false;
            var errorMessage = string.Empty;

            var cacheKey = GetCacheKey(mpAppId);
            var token = _cacheManager.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(token))
            {
                var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={mpAppId}&secret={mpAppSecret}";
                string result;
                (success, result, errorMessage) = await RestUtils.GetAsync<string>(url);
                if (success)
                {
                    if (StringUtils.Contains(result, "errcode"))
                    {
                        var jsonError = TranslateUtils.JsonDeserialize<JsonResult>(result);
                        // https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_access_token.html
                        if (jsonError.errcode == 40164)
                        {
                            var startIndex = jsonError.errmsg.IndexOf("invalid ip ", StringComparison.Ordinal) + 11;
                            var endIndex = jsonError.errmsg.IndexOf(" ipv6", StringComparison.Ordinal);
                            var ip = jsonError.errmsg.Substring(startIndex, endIndex - startIndex);
                            errorMessage = $"调用接口的IP地址不在白名单中，请进入微信公众平台，将本服务器的IP地址 {ip} 添加至白名单，如果已配置，请等待 10 分钟左右再试。";
                        }
                        else
                        {
                            errorMessage = $"API 调用发生错误：{jsonError.errmsg}";
                        }

                        await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.GetAccessTokenAsync");
                    }
                    else
                    {
                        var accessToken = TranslateUtils.JsonDeserialize<AccessToken>(result);
                        token = accessToken.access_token;
                        if (!string.IsNullOrEmpty(token))
                        {
                            var minutes = accessToken.expires_in / 60 - 5;
                            _cacheManager.AddOrUpdateAbsolute(cacheKey, token, minutes);
                        }
                    }
                }
                else
                {
                    await _errorLogRepository.AddErrorLogAsync(new Exception(errorMessage), "WxManager.GetAccessTokenAsync");
                }
            }

            return (success, token, errorMessage);
        }

        // public async Task<(bool Success, string AccessToken, string ErrorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret)
        // {
        //     var success = false;
        //     var errorMessage = string.Empty;
        //     string token = null;

        //     try
        //     {
        //         token = await AccessTokenContainer.TryGetAccessTokenAsync(mpAppId, mpAppSecret);
        //         success = true;
        //     }
        //     catch (ErrorJsonResultException ex)
        //     {
        //         if (ex.JsonResult.errcode == ReturnCode.调用接口的IP地址不在白名单中)
        //         {
        //             var startIndex = ex.JsonResult.errmsg.IndexOf("invalid ip ", StringComparison.Ordinal) + 11;
        //             var endIndex = ex.JsonResult.errmsg.IndexOf(" ipv6", StringComparison.Ordinal);
        //             var ip = ex.JsonResult.errmsg.Substring(startIndex, endIndex - startIndex);
        //             errorMessage = $"调用接口的IP地址不在白名单中，请进入微信公众平台，将本服务器的IP地址 {ip} 添加至白名单，如果已配置，请等待 10 分钟左右再试。";
        //         }
        //         else
        //         {
        //             errorMessage = $"API 调用发生错误：{ex.JsonResult.errmsg}";
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         errorMessage = $"执行过程发生错误：{ex.Message}";
        //     }

        //     return (success, token, errorMessage);
        // }

        public async Task<(bool success, string ticket, string errorMessage)> GetJsApiTicketAsync(string mpAppId, string mpAppSecret)
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
    }
}
