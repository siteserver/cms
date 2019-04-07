using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cloud
{
    [RoutePrefix("pages/cloud/authorize")]
    public class PagesAuthorizeController : ApiController
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

                var userName = Request.GetPostString("userName");
                var accessToken = Request.GetPostString("accessToken");
                var expiresAt = Request.GetPostDateTime("expiresAt", DateTime.Now);

                //ConfigManager.SystemConfigInfo.IsCloudLoggin = true; ConfigManager.SystemConfigInfo.CloudUserName = userName;
                //ConfigManager.SystemConfigInfo.CloudAccessToken = accessToken;
                //ConfigManager.SystemConfigInfo.CloudExpiresAt = expiresAt;

                DataProvider.Config.Update(ConfigManager.Instance);

                LogUtils.AddAdminLog(rest.AdminName, "云服务账号登录", $"UserName:{userName}");

                return Ok(new
                {
                    Value = userName
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
