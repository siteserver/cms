using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Wx;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteActionsEditTag)]
        public async Task<ActionResult<EditTagResult>> EditTag([FromBody] EditTagRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxUsers))
            {
                return Unauthorized();
            }

            List<WxUserTag> tags = null;

            var (success, token, _) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                await _wxManager.UpdateUserTag(token, request.TagId, request.TagName);
                tags = await _wxManager.GetUserTagsAsync(token);
            }

            return new EditTagResult
            {
                Tags = tags
            };
        }
    }
}
