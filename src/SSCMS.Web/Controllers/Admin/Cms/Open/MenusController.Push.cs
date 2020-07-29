using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Extensions;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class MenusController
    {
        [HttpPost, Route(RouteActionsPush)]
        public async Task<ActionResult<PushResult>> Push([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.OpenMenus))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _openManager.GetWxAccessTokenAsync(request.SiteId);

            if (success)
            {
                try
                {
                    await _openManager.PushMenuAsync(token, request.SiteId);
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
