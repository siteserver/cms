using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteActionsRemark)]
        public async Task<ActionResult<BoolResult>> Remark([FromBody] RemarkRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.WxUsers))
            {
                return Unauthorized();
            }

            var (success, token, _) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                await _wxManager.UpdateUserRemarkAsync(token, request.OpenId, request.Remark);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
