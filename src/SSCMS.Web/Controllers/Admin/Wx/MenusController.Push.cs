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
        [HttpPost, Route(RoutePush)]
        public async Task<ActionResult<BoolResult>> Push([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);

            if (!success)
            {
                return this.Error(errorMessage);
            }

            try
            {
                await _wxManager.PushMenuAsync(token, request.SiteId);
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
