using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ChatSendController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxChat))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            if (request.MaterialType == MaterialType.Text)
            {
                await _wxManager.CustomSendTextAsync(token, request.OpenId, request.SiteId, request.Text);
            }
            else if (request.MaterialType == MaterialType.Message)
            {
                await _wxManager.CustomSendMessageAsync(token, request.OpenId, request.SiteId, request.MaterialId, null);
            }
            else if (request.MaterialType == MaterialType.Image)
            {
                await _wxManager.CustomSendImageAsync(token, request.OpenId, request.SiteId, request.MaterialId, null);
            }
            else if (request.MaterialType == MaterialType.Audio)
            {
                await _wxManager.CustomSendAudioAsync(token, request.OpenId, request.SiteId, request.MaterialId, null);
            }
            else if (request.MaterialType == MaterialType.Video)
            {
                await _wxManager.CustomSendVideoAsync(token, request.OpenId, request.SiteId, request.MaterialId, null);
            }

            var chats = await _wxChatRepository.GetChatsAsyncByOpenId(request.SiteId, request.OpenId);

            return new SubmitResult
            {
                Chats = chats
            };
        }
    }
}
