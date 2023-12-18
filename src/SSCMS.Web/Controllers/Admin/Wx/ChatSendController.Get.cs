using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Utils;


namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ChatSendController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxChat))
            {
                return Unauthorized();
            }

            WxUser user = null;
            List<WxChat> chats = null;

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isWxEnabled = await _wxManager.IsEnabledAsync(site);

            if (isWxEnabled)
            {
                var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                user = await _wxManager.GetUserAsync(token, request.OpenId);
                chats = await _wxChatRepository.GetChatsAsyncByOpenId(request.SiteId, request.OpenId);
            }

            return new GetResult
            {
                IsWxEnabled = isWxEnabled,
                User = user,
                Chats = chats
            };
        }
    }
}
