using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;

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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    return Unauthorized();
                }

                var userName = request.GetPostString("userName");
                var accessToken = request.GetPostString("accessToken");
                var expiresAt = request.GetPostDateTime("expiresAt", DateTime.Now);

                ConfigManager.SystemConfigInfo.IsCloudLoggin = true; ConfigManager.SystemConfigInfo.CloudUserName = userName;
                ConfigManager.SystemConfigInfo.CloudAccessToken = accessToken;
                ConfigManager.SystemConfigInfo.CloudExpiresAt = expiresAt;

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("云服务账号登录", $"UserName:{userName}");

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
