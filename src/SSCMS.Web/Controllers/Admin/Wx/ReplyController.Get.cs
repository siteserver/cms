using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Wx;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.WxReply))
            {
                return Unauthorized();
            }

            List<WxUserTag> tags = null;
            var total = 0;
            var count = 0;
            List<WxUser> users = null;

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                Tags = tags,
                Total = total,
                Count = count,
                Users = users
            };
        }
    }
}
