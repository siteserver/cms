using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteActionsDeleteTag)]
        public async Task<ActionResult<DeleteTagResult>> DeleteTag([FromBody] DeleteTagRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxUsers))
            {
                return Unauthorized();
            }

            List<WxUserTag> tags = null;

            var (success, token, _) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                await _wxManager.DeleteUserTag(token, request.TagId);
                tags = await _wxManager.GetUserTagsAsync(token);
            }

            return new DeleteTagResult
            {
                Tags = tags
            };
        }
    }
}
