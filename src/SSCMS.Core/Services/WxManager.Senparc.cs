using System;
using System.Threading.Tasks;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using SSCMS.Enums;

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public string GetNonceStr()
        {
            return JSSDKHelper.GetNoncestr();
        }

        public string GetTimestamp()
        {
            return JSSDKHelper.GetTimestamp();
        }

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
    }
}
