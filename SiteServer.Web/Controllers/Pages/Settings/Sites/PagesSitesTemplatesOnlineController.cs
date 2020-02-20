using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages.Settings.Sites
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/sitesTemplatesOnline")]
    public class PagesSitesTemplatesOnlineController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesTemplatesOnline))
                {
                    return Unauthorized();
                }

                var siteAddPermission =
                    await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSitesAdd);

                return Ok(new
                {
                    Value = true,
                    SiteAddPermission = siteAddPermission
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
