using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userConfig")]
    public class PagesUserConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.Instance.IsUserRegistrationAllowed = Request.GetPostBool("isUserRegistrationAllowed");
                ConfigManager.Instance.IsUserRegistrationChecked = Request.GetPostBool("isUserRegistrationChecked");
                ConfigManager.Instance.IsUserUnRegistrationAllowed = Request.GetPostBool("isUserUnRegistrationAllowed");
                ConfigManager.Instance.UserPasswordMinLength = Request.GetPostInt("userPasswordMinLength");
                ConfigManager.Instance.UserPasswordRestriction = Request.GetPostString("userPasswordRestriction");
                ConfigManager.Instance.UserRegistrationMinMinutes = Request.GetPostInt("userRegistrationMinMinutes");
                ConfigManager.Instance.IsUserLockLogin = Request.GetPostBool("isUserLockLogin");
                ConfigManager.Instance.UserLockLoginCount = Request.GetPostInt("userLockLoginCount");
                ConfigManager.Instance.UserLockLoginType = Request.GetPostString("userLockLoginType");
                ConfigManager.Instance.UserLockLoginHours = Request.GetPostInt("userLockLoginHours");

                DataProvider.Config.Update(ConfigManager.Instance);

                LogUtils.AddAdminLog(rest.AdminName, "修改用户设置");

                return Ok(new
                {
                    Value = ConfigManager.Instance
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
