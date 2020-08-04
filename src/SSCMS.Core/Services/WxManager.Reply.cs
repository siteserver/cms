using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Services
{
    ////群发媒体文件时传入mediaId,群发文本消息时传入content,群发卡券时传入cardId
    public partial class WxManager
    {
        public async Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, int ruleId)
        {
            var messages = new List<WxReplyMessage>();
            var dbMessages = await _wxReplyMessageRepository.GetMessagesAsync(siteId, ruleId);

            foreach (var dbMessage in dbMessages)
            {
                var message = await GetMessageAsync(dbMessage);
                if (message != null)
                {
                    messages.Add(message);
                }
            }

            return messages;
        }

        public async Task<WxReplyMessage> GetMessageAsync(int siteId, int messageId)
        {
            var dbMessage = await _wxReplyMessageRepository.GetMessageAsync(siteId, messageId);

            return await GetMessageAsync(dbMessage);
        }

        private async Task<WxReplyMessage> GetMessageAsync(WxReplyMessage message)
        {
            if (message.MaterialType == MaterialType.Message)
            {
                var material = await _materialMessageRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.Items = material.Items;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Image)
            {
                var material = await _materialImageRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.Image = material;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Audio)
            {
                var material = await _materialAudioRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.Audio = material;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Video)
            {
                var material = await _materialVideoRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.Video = material;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Text)
            {
                return message;
            }

            return null;
        }
    }
}
