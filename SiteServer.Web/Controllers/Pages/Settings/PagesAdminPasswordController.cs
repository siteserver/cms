using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminPassword")]
    public class PagesAdminPasswordController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var userId = request.GetQueryInt("userId");
                if (userId == 0) userId = request.AdminId;
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
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
                var request = new AuthenticatedRequest();
                var userId = request.GetQueryInt("userId");
                if (userId == 0) userId = request.AdminId;
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var password = request.GetPostString("password");

                if (!DataProvider.AdministratorDao.ChangePassword(adminInfo, password, out var errorMessage))
                {
                    return BadRequest($"更改密码失败：{errorMessage}");
                }

                request.AddAdminLog("重设管理员密码", $"管理员:{adminInfo.UserName}");

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
