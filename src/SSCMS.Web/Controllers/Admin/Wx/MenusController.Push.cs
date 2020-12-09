using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class MenusController
    {
        [HttpPost, Route(RouteActionsPush)]
        public async Task<ActionResult<PushResult>> Push([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);

            if (success)
            {
                try
                {
                    await _wxManager.PushMenuAsync(token, request.SiteId);
                }
                catch (Exception ex)
                {
                    return this.Error(ex.Message);
                }
            }

            return new PushResult
            {
                Success = success,
                ErrorMessage = errorMessage
            };
        }
    }
}
