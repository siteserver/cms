using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/siteTemplateOnline")]
    public class PagesSiteTemplateOnlineController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsSiteTemplatesOnline))
                {
                    return Unauthorized();
                }

                var siteAddPermission =
                    request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsSiteAdd);

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
