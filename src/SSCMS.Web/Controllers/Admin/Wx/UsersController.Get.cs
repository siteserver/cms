using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Wx;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int siteId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId, AuthTypes.SitePermissions.WxAccount))
            {
                return Unauthorized();
            }

            IEnumerable<WxUserTag> tags = null;
            List<WxUser> users = null;

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(siteId);
            if (success)
            {
                tags = await _wxManager.GetUserTagsAsync(token);
                users = await _wxManager.GetUsersAsync(token);
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                Tags = tags,
                Users = users
            };
        }
    }
}
