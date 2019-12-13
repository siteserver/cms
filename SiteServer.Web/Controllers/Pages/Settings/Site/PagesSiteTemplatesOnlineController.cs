using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages.Settings.Site
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/siteTemplatesOnline")]
    public class PagesSiteTemplatesOnlineController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSiteTemplatesOnline))
                {
                    return Unauthorized();
                }

                var siteAddPermission =
                    await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSiteAdd);

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
