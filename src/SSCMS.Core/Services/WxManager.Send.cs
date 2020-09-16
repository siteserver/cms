using System;
using System.Threading.Tasks;
using Datory;
using Senparc.Weixin.MP.AdvancedAPIs;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    ////群发媒体文件时传入mediaId,群发文本消息时传入content,群发卡券时传入cardId
    public partial class WxManager
    {
        public async Task PreviewSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, string wxName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(wxName)) return;
            
            await GroupMessageApi.SendGroupMessagePreviewAsync(accessTokenOrAppId, GetGroupMessageType(materialType), value,
                null, StringUtils.Trim(wxName));
        }

        public async Task MassSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, bool isToAll, string tagId, DateTime? runOnceAt)
        {
            if (runOnceAt.HasValue)
            {
                _taskManager.RunOnceAt(async () =>
                {
                    await GroupMessageApi.SendGroupMessageByTagIdAsync(accessTokenOrAppId, tagId, value,
                        GetGroupMessageType(materialType), isToAll);
                }, runOnceAt.Value);
            }
            else
            {
                await GroupMessageApi.SendGroupMessageByTagIdAsync(accessTokenOrAppId, tagId, value,
                    GetGroupMessageType(materialType), isToAll);
            }
        }

        public async Task CustomSendAsync(string accessTokenOrAppId, string openId, WxReplyMessage message)
        {
            if (message.MaterialType == MaterialType.Message)
            {
                await CustomSendMessageAsync(accessTokenOrAppId, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
            else if (message.MaterialType == MaterialType.Text)
            {
                await CustomSendTextAsync(accessTokenOrAppId, openId, message.SiteId, message.Text);
            }
            else if (message.MaterialType == MaterialType.Image)
            {
                await CustomSendImageAsync(accessTokenOrAppId, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
            else if (message.MaterialType == MaterialType.Audio)
            {
                await CustomSendAudioAsync(accessTokenOrAppId, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
            else if (message.MaterialType == MaterialType.Video)
            {
                await CustomSendVideoAsync(accessTokenOrAppId, openId, message.SiteId, message.MaterialId, message.MediaId);
            }
        }

        public async Task CustomSendTextAsync(string accessTokenOrAppId, string openId, int siteId, string text)
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

            await CustomApi.SendTextAsync(accessTokenOrAppId, openId, text);
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

            await CustomApi.SendImageAsync(accessTokenOrAppId, openId, mediaId);
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
    }
}
