using System;
using System.Threading.Tasks;
using Datory;
using Senparc.Weixin.MP.AdvancedAPIs;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    // https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Service_Center_messages.html#%E5%AE%A2%E6%9C%8D%E6%8E%A5%E5%8F%A3-%E5%8F%91%E6%B6%88%E6%81%AF

    public partial class WxManager
    {
        public async Task PreviewSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, string wxName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(wxName)) return;
            
            await GroupMessageApi.SendGroupMessagePreviewAsync(accessTokenOrAppId, GetGroupMessageType(materialType), value,
                null, StringUtils.Trim(wxName));
        }

        public async Task MassSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, DateTime? runOnceAt)
        {
            if (runOnceAt.HasValue)
            {
                _taskManager.RunOnceAt(async () =>
                {
                    await GroupMessageApi.SendGroupMessageByTagIdAsync(accessTokenOrAppId, string.Empty, value,
                        GetGroupMessageType(materialType), true);
                }, runOnceAt.Value);
            }
            else
            {
                await GroupMessageApi.SendGroupMessageByTagIdAsync(accessTokenOrAppId, string.Empty, value,
                    GetGroupMessageType(materialType), true);
            }
        }

        public async Task CustomSendAsync(string accessToken, string openId, WxReplyMessage message)
        {
            if (message.MaterialType == MaterialType.Message)
            {
                await CustomSendMessageAsync(accessToken, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
            else if (message.MaterialType == MaterialType.Text)
            {
                await CustomSendTextAsync(accessToken, openId, message.SiteId, message.Text);
            }
            else if (message.MaterialType == MaterialType.Image)
            {
                await CustomSendImageAsync(accessToken, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
            else if (message.MaterialType == MaterialType.Audio)
            {
                await CustomSendAudioAsync(accessToken, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
            else if (message.MaterialType == MaterialType.Video)
            {
                await CustomSendVideoAsync(accessToken, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
        }

        public async Task CustomSendMessageAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId))
            {
                mediaId = await PushMaterialAsync(accessTokenOrAppId, MaterialType.Message, materialId);
            }

            await _wxChatRepository.ReplyAdd(new WxChat
            {
                SiteId = siteId,
                OpenId = openId,
                IsReply = true,
                MaterialType = MaterialType.Message,
                MaterialId = materialId,
                Text = MaterialType.Message.GetDisplayName()
            });

            await CustomApi.SendMpNewsAsync(accessTokenOrAppId, openId, mediaId);
        }

        public async Task CustomSendImageAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId))
            {
                mediaId = await PushMaterialAsync(accessTokenOrAppId, MaterialType.Image, materialId);
            }

            await _wxChatRepository.ReplyAdd(new WxChat
            {
                SiteId = siteId,
                OpenId = openId,
                IsReply = true,
                MaterialType = MaterialType.Image,
                MaterialId = materialId,
                Text = MaterialType.Image.GetDisplayName()
            });

            var body = $@"
{{
    ""touser"":""{openId}"",
    ""msgtype"":""image"",
    ""image"":
    {{
      ""media_id"":""{mediaId}""
    }}
}}";

            await CustomSendAsync(accessTokenOrAppId, body);
        }

        public async Task CustomSendAudioAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId))
            {
                mediaId = await PushMaterialAsync(accessTokenOrAppId, MaterialType.Audio, materialId);
            }

            await _wxChatRepository.ReplyAdd(new WxChat
            {
                SiteId = siteId,
                OpenId = openId,
                IsReply = true,
                MaterialType = MaterialType.Audio,
                MaterialId = materialId,
                Text = MaterialType.Audio.GetDisplayName()
            });

            await CustomApi.SendVoiceAsync(accessTokenOrAppId, openId, mediaId);
        }

        public async Task CustomSendVideoAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId))
            {
                mediaId = await PushMaterialAsync(accessTokenOrAppId, MaterialType.Video, materialId);
            }

            var video = await _materialVideoRepository.GetAsync(materialId);

            await _wxChatRepository.ReplyAdd(new WxChat
            {
                SiteId = siteId,
                OpenId = openId,
                IsReply = true,
                MaterialType = MaterialType.Video,
                MaterialId = materialId,
                Text = MaterialType.Video.GetDisplayName()
            });

            await CustomApi.SendVideoAsync(accessTokenOrAppId, openId, mediaId, video.Title, video.Description);
        }

        public async Task CustomSendTextAsync(string accessToken, string openId, int siteId, string text)
        {
            await _wxChatRepository.ReplyAdd(new WxChat
            {
                SiteId = siteId,
                OpenId = openId,
                IsReply = true,
                MaterialType = MaterialType.Text,
                MaterialId = 0,
                Text = text
            });

            var body = $@"
{{
    ""touser"":""{openId}"",
    ""msgtype"":""text"",
    ""text"":
    {{
         ""content"":""{text}""
    }}
}}";

            await CustomSendAsync(accessToken, body);
        }

        // https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Service_Center_messages.html#%E5%AE%A2%E6%9C%8D%E6%8E%A5%E5%8F%A3-%E5%8F%91%E6%B6%88%E6%81%AF
        public async Task<(bool success, string errorMessage)> CustomSendAsync(string accessToken, string body)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={accessToken}";
            var (success, result, errorMessage) = await RestUtils.PostStringAsync(url, body);

            if (success)
            {
                var json = TranslateUtils.JsonDeserialize<JsonResult>(result);
                if (json.errcode != 0)
                {
                    success = false;
                    errorMessage = $"API 调用发生错误：{json.errmsg}";

                    await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.SendTextAsync");
                }
            }
            else
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(errorMessage), "WxManager.SendTextAsync");
            }

            return (success, errorMessage);
        }
    }
}
