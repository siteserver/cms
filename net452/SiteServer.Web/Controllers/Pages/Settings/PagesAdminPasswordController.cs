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
    [RoutePrefix("pages/settings/adminPassword")]
    public class PagesAdminPasswordController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                var userId = Request.GetQueryInt("userId");
                if (!rest.IsAdminLoggin) return Unauthorized();
                var adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                if (adminInfo == null) return NotFound();
                if (rest.AdminId != userId &&
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = adminInfo
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
                var userId = Request.GetQueryInt("userId");
                if (!rest.IsAdminLoggin) return Unauthorized();
                var adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                if (adminInfo == null) return NotFound();
                if (rest.AdminId != userId &&
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var password = Request.GetPostString("password");

                if (!DataProvider.Administrator.ChangePassword(adminInfo, password, out var errorMessage))
                {
                    return BadRequest($"更改密码失败：{errorMessage}");
                }

                LogUtils.AddAdminLog(rest.AdminName, "重设管理员密码", $"管理员:{adminInfo.UserName}");

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
