using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Services
{
    public interface IWxManager
    {
        string GetNonceStr();

        string GetTimestamp();

        Task<(bool success, string ticket, string errorMessage)> GetJsApiTicketAsync(string mpAppId, string mpAppSecret);

        string GetJsApiSignature(string ticket, string nonceStr, string timestamp, string url);

        Task<bool> IsEnabledAsync(Site site);

        Task<(bool success, string mediaId, string errorMessage)> AddMaterialAsync(string accessToken, WxMaterialType materialType, string filePath);

        Task<(bool success, string mediaId, string errorMessage)> DraftAddAsync(string accessToken, int messageId);

        Task<(bool success, string publishId, string errorMessage)> FreePublishSubmitAsync(string accessToken, string mediaId);

        Task<string> ReplyAsync(WxAccount account, string fromUserName, string toUserName, WxReplyMessage replyMessage, string timestamp, string nonce);

        Task<(bool success, string errorMessage)> PreviewAsync(string accessToken, string mediaId, string wxName);

        Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, string content, int defaultMessageId);

        Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, int ruleId);

        Task<WxReplyMessage> GetMessageAsync(int siteId, int messageId);

        Task PreviewSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, string wxName);

        Task MassSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, DateTime? sendAt = null);

        Task CustomSendAsync(string accessTokenOrAppId, string openId, WxReplyMessage message);

        Task CustomSendTextAsync(string accessTokenOrAppId, string openId, int siteId, string text);

        Task CustomSendMessageAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId, bool retry = true);

        Task CustomSendImageAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId, bool retry = true);

        Task CustomSendAudioAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId, bool retry = true);

        Task CustomSendVideoAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId, bool retry = true);

        Task<string> PushMaterialAsync(string accessTokenOrAppId, MaterialType materialType, int materialId);

        Task PullMenuAsync(string accessTokenOrAppId, int siteId);

        Task PushMenuAsync(string accessTokenOrAppId, int siteId);

        string GetErrorUnAuthenticated(WxAccount account);
        
        Task<WxAccount> GetAccountAsync(int siteId);

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(WxAccount account);
        
        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(int siteId);

        Task<(bool success, string accessToken, string errorMessage)> GetAccessTokenAsync(string mpAppId, string mpAppSecret);
    }
}
