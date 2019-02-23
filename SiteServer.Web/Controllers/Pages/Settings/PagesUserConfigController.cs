using System;
using System.Web.Http;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;

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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemExtend
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.Instance.SystemExtend.IsUserRegistrationAllowed = rest.GetPostBool("isUserRegistrationAllowed");
                ConfigManager.Instance.SystemExtend.IsUserRegistrationChecked = rest.GetPostBool("isUserRegistrationChecked");
                ConfigManager.Instance.SystemExtend.IsUserUnRegistrationAllowed = rest.GetPostBool("isUserUnRegistrationAllowed");
                ConfigManager.Instance.SystemExtend.UserPasswordMinLength = rest.GetPostInt("userPasswordMinLength");
                ConfigManager.Instance.SystemExtend.UserPasswordRestriction = rest.GetPostString("userPasswordRestriction");
                ConfigManager.Instance.SystemExtend.UserRegistrationMinMinutes = rest.GetPostInt("userRegistrationMinMinutes");
                ConfigManager.Instance.SystemExtend.IsUserLockLogin = rest.GetPostBool("isUserLockLogin");
                ConfigManager.Instance.SystemExtend.UserLockLoginCount = rest.GetPostInt("userLockLoginCount");
                ConfigManager.Instance.SystemExtend.UserLockLoginType = rest.GetPostString("userLockLoginType");
                ConfigManager.Instance.SystemExtend.UserLockLoginHours = rest.GetPostInt("userLockLoginHours");

                DataProvider.Config.Update(ConfigManager.Instance);

                rest.AddAdminLog("修改用户设置");

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemExtend
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
