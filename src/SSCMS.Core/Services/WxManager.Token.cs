using System;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/getStableAccessToken.html

namespace SSCMS.Core.Services
{
    public partial class WxManager : IWxManager
    {
        private static string GetCacheKey(string mpAppId)
        {
            return $"{nameof(WxManager)}.{mpAppId}";
        }

        public string GetErrorUnAuthenticated(WxAccount account)
        {
            return $@"您的公众号类型为{account.MpType.GetDisplayName()}，未获得微信接口授权，可以访问 <a href=""https://developers.weixin.qq.com/doc/offiaccount/Getting_Started/Explanation_of_interface_privileges.html"" target=""_blank"" class=""el-link el-link--primary"">微信官方文档</a> 查看接口权限!";
        }

        public async Task<WxAccount> GetAccountAsync(int siteId)
        {
            return await _wxAccountRepository.GetBySiteIdAsync(siteId);
        }

        public async Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(int siteId)
        {
            var account = await GetAccountAsync(siteId);
            return await GetAccessTokenAsync(account);
        }

        public async Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(WxAccount account)
        {
            return await GetAccessTokenAsync(account.MpAppId, account.MpAppSecret);
        }

        public async Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret)
        {
            if (string.IsNullOrEmpty(mpAppId) || string.IsNullOrEmpty(mpAppSecret))
            {
                return (false, null, "微信公众号 AppId 及 AppSecret 未设置，请到平台账号配置中设置！");
            }

            bool success;
            string result;
            var errorMessage = string.Empty;
            var cacheKey = GetCacheKey(mpAppId);
            var token = _cacheManager.Get<string>(cacheKey);

            if (string.IsNullOrEmpty(token))
            {
                var body = new StableTokenBody
                {
                    grant_type = "client_credential",
                    appid = mpAppId,
                    secret = mpAppSecret,
                    force_refresh = false,
                };
                var url = $"https://api.weixin.qq.com/cgi-bin/stable_token";
                (success, result, errorMessage) = await RestUtils.PostStringAsync(url, TranslateUtils.JsonSerialize(body));

                if (success)
                {
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
            else
            {
                success = true;
            }

            return (success, token, errorMessage);
        }

        // public async Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret)
        // {
        //     bool success;
        //     var errorMessage = string.Empty;
        //     var cacheKey = GetCacheKey(mpAppId);
        //     var token = _cacheManager.Get<string>(cacheKey);

        //     if (string.IsNullOrEmpty(token))
        //     {
        //         var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={mpAppId}&secret={mpAppSecret}";
        //         string result;
        //         (success, result, errorMessage) = await RestUtils.GetStringAsync(url);
        //         if (success)
        //         {
        //             if (StringUtils.Contains(result, "errcode"))
        //             {
        //                 success = false;
        //                 var jsonError = TranslateUtils.JsonDeserialize<JsonResult>(result);
        //                 // https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_access_token.html
        //                 if (jsonError.errcode == 40164)
        //                 {
        //                     var startIndex = jsonError.errmsg.IndexOf("invalid ip ", StringComparison.Ordinal) + 11;
        //                     var endIndex = jsonError.errmsg.IndexOf(" ipv6", StringComparison.Ordinal);
        //                     var ip = jsonError.errmsg.Substring(startIndex, endIndex - startIndex);
        //                     errorMessage = $"调用接口的IP地址不在白名单中，请进入微信公众平台，将本服务器的IP地址 {ip} 添加至白名单，如果已配置，请等待 10 分钟左右再试。";
        //                 }
        //                 else
        //                 {
        //                     errorMessage = $"API 调用发生错误：{jsonError.errmsg}";
        //                 }

        //                 await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.GetAccessTokenAsync");
        //             }
        //             else
        //             {
        //                 var accessToken = TranslateUtils.JsonDeserialize<AccessToken>(result);
        //                 token = accessToken.access_token;
        //                 if (!string.IsNullOrEmpty(token))
        //                 {
        //                     var minutes = accessToken.expires_in / 60 - 5;
        //                     _cacheManager.AddOrUpdateAbsolute(cacheKey, token, minutes);
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             await _errorLogRepository.AddErrorLogAsync(new Exception(errorMessage), "WxManager.GetAccessTokenAsync");
        //         }
        //     }
        //     else
        //     {
        //         success = true;
        //     }

        //     return (success, token, errorMessage);
        // }

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
    }
}
