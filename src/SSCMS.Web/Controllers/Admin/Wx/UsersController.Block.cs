using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteActionsBlock)]
        public async Task<ActionResult<BoolResult>> Block([FromBody] BlockRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxUsers))
            {
                return Unauthorized();
            }

            var (success, token, _) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                await _wxManager.UserBatchBlackListAsync(token, request.OpenIds);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
