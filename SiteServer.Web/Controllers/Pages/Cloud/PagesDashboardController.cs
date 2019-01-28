using System;
using System.Web.Http;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.API.Controllers.Pages.Cloud
{
    [RoutePrefix("pages/cloud/dashboard")]
    public class PagesDashboardController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin || !request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                if (!ConfigManager.SystemConfigInfo.IsCloudLoggin || ConfigManager.SystemConfigInfo.CloudExpiresAt < DateTime.Now)
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.SystemConfigInfo.CloudUserName,
                    AccessToken = ConfigManager.SystemConfigInfo.CloudAccessToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
