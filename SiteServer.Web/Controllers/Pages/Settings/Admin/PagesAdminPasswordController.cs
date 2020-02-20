using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminPassword")]
    public class PagesAdminPasswordController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var userId = request.GetQueryInt("userId");
                if (userId == 0) userId = request.AdminId;
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    ! await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                var userId = request.GetQueryInt("userId");
                if (userId == 0) userId = request.AdminId;
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
                {
                    return Unauthorized();
                }

                var password = request.GetPostString("password");
                var valid = await DataProvider.AdministratorRepository.ChangePasswordAsync(adminInfo, password);
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
