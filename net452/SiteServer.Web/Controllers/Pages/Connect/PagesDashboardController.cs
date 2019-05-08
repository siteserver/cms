using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

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
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin || !rest.AdminPermissions.IsSuperAdmin())
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
