using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    ////群发媒体文件时传入mediaId,群发文本消息时传入content,群发卡券时传入cardId
    public partial class WxManager
    {
        private static bool IsMatch(WxReplyKeyword keyword, string content)
        {
            if (keyword.Exact)
            {
                return StringUtils.EqualsIgnoreCase(keyword.Text, content);
            }

            return StringUtils.ContainsIgnoreCase(keyword.Text, content) ||
                   StringUtils.ContainsIgnoreCase(content, keyword.Text);
        }

        public async Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, string content, int defaultMessageId)
        {
            List<WxReplyMessage> messages = null;

            var keywords = await _wxReplyKeywordRepository.GetKeywordsAsync(siteId);
            var keyword = keywords.FirstOrDefault(x => IsMatch(x, content));
            if (keyword != null)
            {
                var rule = await _wxReplyRuleRepository.GetAsync(keyword.RuleId);
                if (rule != null)
                {
                    var allMessages = await GetMessagesAsync(siteId, keyword.RuleId);
                    if (rule.Random)
                    {
                        var index = StringUtils.GetRandomInt(0, allMessages.Count - 1);
                        messages = new List<WxReplyMessage>
                        {
                            allMessages[index]
                        };
                    }
                    else
                    {
                        messages = allMessages;
                    }
                }
            }
            else if (defaultMessageId > 0)
            {
                var message = await GetMessageAsync(siteId, defaultMessageId);
                if (message != null)
                {
                    messages = new List<WxReplyMessage>
                    {
                        message
                    };
                }
            }

            return messages;
        }

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
            if (siteId == 0 || messageId == 0) return null;

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
                    message.MediaId = material.MediaId;
                    message.Items = material.Items;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Image)
            {
                var material = await _materialImageRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.MediaId = material.MediaId;
                    message.Image = material;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Audio)
            {
                var material = await _materialAudioRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.MediaId = material.MediaId;
                    message.Audio = material;
                    return message;
                }
            }
            else if (message.MaterialType == MaterialType.Video)
            {
                var material = await _materialVideoRepository.GetAsync(message.MaterialId);
                if (material != null)
                {
                    message.MediaId = material.MediaId;
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
