using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;

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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                ConfigManager.SystemConfigInfo.IsCloudLoggin = false; ConfigManager.SystemConfigInfo.CloudUserName = string.Empty;
                ConfigManager.SystemConfigInfo.CloudAccessToken = string.Empty;
                ConfigManager.SystemConfigInfo.CloudExpiresAt = DateTime.Now;

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("云服务账号登出");

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
