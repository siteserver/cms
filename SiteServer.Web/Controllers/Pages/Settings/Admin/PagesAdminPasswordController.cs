using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminPassword")]
    public class PagesAdminPasswordController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                var userId = request.GetQueryInt("userId");
                if (userId == 0) userId = request.AdminId;
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = await AdminManager.GetByUserIdAsync(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    ! await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Admin))
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
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                var userId = request.GetQueryInt("userId");
                if (userId == 0) userId = request.AdminId;
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = await AdminManager.GetByUserIdAsync(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var password = request.GetPostString("password");
                var valid = await DataProvider.AdministratorDao.ChangePasswordAsync(adminInfo, password);
                if (!valid.IsValid)
                {
                    return BadRequest($"更改密码失败：{valid.ErrorMessage}");
                }

                await request.AddAdminLogAsync("重设管理员密码", $"管理员:{adminInfo.UserName}");

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
