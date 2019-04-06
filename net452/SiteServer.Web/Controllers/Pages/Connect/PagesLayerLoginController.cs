using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;

namespace SiteServer.API.Controllers.Pages.Cloud
{
    [RoutePrefix("pages/cloud/layerLogin")]
    public class PagesLayerLoginController : ApiController
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

                var userName = rest.GetPostString("userName");
                var accessToken = rest.GetPostString("accessToken");
                var expiresAt = rest.GetPostDateTime("expiresAt", DateTime.Now);

                //ConfigManager.SystemConfigInfo.IsCloudLoggin = true; ConfigManager.SystemConfigInfo.CloudUserName = userName;
                //ConfigManager.SystemConfigInfo.CloudAccessToken = accessToken;
                //ConfigManager.SystemConfigInfo.CloudExpiresAt = expiresAt;

                DataProvider.Config.Update(ConfigManager.Instance);

                rest.AddAdminLog("云服务账号登录", $"UserName:{userName}");

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
