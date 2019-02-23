using System;
using System.Web.Http;

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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin || !rest.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                //if (!ConfigManager.SystemConfigInfo.IsCloudLoggin || ConfigManager.SystemConfigInfo.CloudExpiresAt < DateTime.Now)
                //{
                //    return Unauthorized();
                //}

                //return Ok(new
                //{
                //    Value = ConfigManager.SystemConfigInfo.CloudUserName,
                //    AccessToken = ConfigManager.SystemConfigInfo.CloudAccessToken
                //});

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
