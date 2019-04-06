using System;
using System.Web.Http;

namespace SiteServer.API.Controllers.Pages.Cloud
{
    [RoutePrefix("pages/cloud/logout")]
    public class PagesLogoutController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                //ConfigManager.SystemConfigInfo.IsCloudLoggin = false; ConfigManager.SystemConfigInfo.CloudUserName = string.Empty;
                //ConfigManager.SystemConfigInfo.CloudAccessToken = string.Empty;
                //ConfigManager.SystemConfigInfo.CloudExpiresAt = DateTime.Now;

                //DataProvider.ConfigDao.UpdateObject(ConfigManager.Instance);

                rest.AddAdminLog("云服务账号登出");

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
