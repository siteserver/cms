using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;

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

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                user = await _wxManager.GetUserAsync(token, request.OpenId);
                chats = await _wxChatRepository.GetChatsAsyncByOpenId(request.SiteId, request.OpenId);
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                User = user,
                Chats = chats
            };
        }
    }
}
