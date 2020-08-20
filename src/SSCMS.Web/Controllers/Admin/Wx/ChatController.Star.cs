using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ChatController
    {
        [HttpPost, Route(RouteActionsStar)]
        public async Task<ActionResult<BoolResult>> Star([FromBody] StarRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.WxChat))
            {
                return Unauthorized();
            }

            await _wxChatRepository.Star(request.SiteId, request.ChatId, request.Star);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
