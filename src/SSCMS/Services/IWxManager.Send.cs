using System;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task PreviewSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, string wxName);

        Task MassSendAsync(string accessTokenOrAppId, MaterialType materialType, string value, bool isToAll, string tagId, DateTime? sendAt = null);

        Task CustomSendAsync(string accessTokenOrAppId, string openId, WxReplyMessage message);

        Task CustomSendTextAsync(string accessTokenOrAppId, string openId, int siteId, string text);

        Task CustomSendMessageAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId);

        Task CustomSendImageAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId);

        Task CustomSendAudioAsync(string accessTokenOrAppId, string openId, int siteId, int materialId, string mediaId);

        Task CustomSendVideoAsync(string accessTokenOrAppId, string openId, int siteId, int materialId,
            string mediaId);
    }
}
