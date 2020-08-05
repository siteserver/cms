using System;
using System.Threading.Tasks;
using Senparc.Weixin.MP.AdvancedAPIs;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    ////群发媒体文件时传入mediaId,群发文本消息时传入content,群发卡券时传入cardId
    public partial class WxManager
    {
        public async Task PreviewSendAsync(string token, MaterialType materialType, string value, string wxName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(wxName)) return;
            
            await GroupMessageApi.SendGroupMessagePreviewAsync(token, GetGroupMessageType(materialType), value,
                null, StringUtils.Trim(wxName));
        }

        public async Task MassSendAsync(string token, MaterialType materialType, string value, bool isToAll, string tagId, DateTime? runOnceAt)
        {
            if (runOnceAt.HasValue)
            {
                _taskManager.RunOnceAt(async () =>
                {
                    await GroupMessageApi.SendGroupMessageByTagIdAsync(token, tagId, value,
                        GetGroupMessageType(materialType), isToAll);
                }, runOnceAt.Value);
            }
            else
            {
                await GroupMessageApi.SendGroupMessageByTagIdAsync(token, tagId, value,
                    GetGroupMessageType(materialType), isToAll);
            }
        }

        public async Task CustomSendAsync(string accessTokenOrAppId, string openId, WxReplyMessage message, bool delay = true)
        {
            if (message.MaterialType == MaterialType.Message)
            {
                await CustomSendMpNewsAsync(accessTokenOrAppId, openId, message.MediaId, delay);
            }
            else if (message.MaterialType == MaterialType.Text)
            {
                await CustomSendTextAsync(accessTokenOrAppId, openId, message.Text, delay);
            }
            else if (message.MaterialType == MaterialType.Image)
            {
                await CustomSendImageAsync(accessTokenOrAppId, openId, message.MediaId, delay);
            }
            else if (message.MaterialType == MaterialType.Audio)
            {
                await CustomSendAudioAsync(accessTokenOrAppId, openId, message.MediaId, delay);
            }
            else if (message.MaterialType == MaterialType.Video)
            {
                await CustomSendVideoAsync(accessTokenOrAppId, openId, message.MediaId, message.Video.Title, message.Video.Description, delay);
            }
        }

        public async Task CustomSendMpNewsAsync(string accessTokenOrAppId, string openId, string mediaId, bool delay = true)
        {
            if (delay)
            {
                _taskManager.RunOnceAt(async () => { await CustomApi.SendMpNewsAsync(accessTokenOrAppId, openId, mediaId); },
                    DateTime.Now.AddSeconds(5));
            }
            else
            {
                await CustomApi.SendMpNewsAsync(accessTokenOrAppId, openId, mediaId);
            }
        }

        public async Task CustomSendTextAsync(string accessTokenOrAppId, string openId, string content, bool delay = true)
        {
            if (delay)
            {
                _taskManager.RunOnceAt(async () => { await CustomApi.SendTextAsync(accessTokenOrAppId, openId, content); },
                    DateTime.Now.AddSeconds(5));
            }
            else
            {
                await CustomApi.SendTextAsync(accessTokenOrAppId, openId, content);
            }
        }

        public async Task CustomSendImageAsync(string accessTokenOrAppId, string openId, string mediaId, bool delay = true)
        {
            if (delay)
            {
                _taskManager.RunOnceAt(
                    async () => { await CustomApi.SendImageAsync(accessTokenOrAppId, openId, mediaId); },
                    DateTime.Now.AddSeconds(5));
            }
            else
            {
                await CustomApi.SendImageAsync(accessTokenOrAppId, openId, mediaId);
            }
        }

        public async Task CustomSendAudioAsync(string accessTokenOrAppId, string openId, string mediaId, bool delay = true)
        {
            if (delay)
            {
                _taskManager.RunOnceAt(
                    async () => { await CustomApi.SendVoiceAsync(accessTokenOrAppId, openId, mediaId); },
                    DateTime.Now.AddSeconds(5));
            }
            else
            {
                await CustomApi.SendVoiceAsync(accessTokenOrAppId, openId, mediaId);
            }
        }

        public async Task CustomSendVideoAsync(string accessTokenOrAppId, string openId, string mediaId, string title, string description, bool delay = true)
        {
            if (delay)
            {
                _taskManager.RunOnceAt(
                    async () => { await CustomApi.SendVideoAsync(accessTokenOrAppId, openId, mediaId, title, description); },
                    DateTime.Now.AddSeconds(5));
            }
            else
            {
                await CustomApi.SendVideoAsync(accessTokenOrAppId, openId, mediaId, title, description);
            }
        }
    }
}
