using System;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task PreviewSendAsync(string token, MaterialType materialType, string value, string wxName);

        Task MassSendAsync(string token, MaterialType materialType, string value, bool isToAll, string tagId, DateTime? sendAt = null);

        Task CustomSendAsync(string accessTokenOrAppId, string openId, WxReplyMessage message, bool delay = true);

        Task CustomSendMpNewsAsync(string accessTokenOrAppId, string openId, string mediaId, bool delay = true);

        Task CustomSendTextAsync(string accessTokenOrAppId, string openId, string content, bool delay = true);

        Task CustomSendImageAsync(string accessTokenOrAppId, string openId, string mediaId, bool delay = true);

        Task CustomSendAudioAsync(string accessTokenOrAppId, string openId, string mediaId, bool delay = true);

        Task CustomSendVideoAsync(string accessTokenOrAppId, string openId, string mediaId, string title,
            string description, bool delay = true);
    }
}
