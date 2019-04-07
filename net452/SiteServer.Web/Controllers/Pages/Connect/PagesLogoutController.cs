using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

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
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.IsSuperAdmin())
                {
                    return Unauthorized();
                }

                //ConfigManager.SystemConfigInfo.IsCloudLoggin = false; ConfigManager.SystemConfigInfo.CloudUserName = string.Empty;
                //ConfigManager.SystemConfigInfo.CloudAccessToken = string.Empty;
                //ConfigManager.SystemConfigInfo.CloudExpiresAt = DateTime.Now;

                //DataProvider.ConfigDao.UpdateObject(ConfigManager.Instance);

                LogUtils.AddAdminLog(rest.AdminName, "云服务账号登出");

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
