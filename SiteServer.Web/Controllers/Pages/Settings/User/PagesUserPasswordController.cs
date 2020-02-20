using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userPassword")]
    public class PagesUserPasswordController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
                {
                    return Unauthorized();
                }

                var userId = request.GetQueryInt("userId");
                var user = await DataProvider.UserRepository.GetByUserIdAsync(userId);
                if (user == null) return NotFound();

                return Ok(new
                {
                    Value = user
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
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
                {
                    return Unauthorized();
                }

                var userId = request.GetQueryInt("userId");
                var user = await DataProvider.UserRepository.GetByUserIdAsync(userId);
                if (user == null) return NotFound();

                var password = request.GetPostString("password");
                var valid = await DataProvider.UserRepository.ChangePasswordAsync(user.Id, password);
                if (!valid.IsValid)
                {
                    return BadRequest($"更改密码失败：{valid.ErrorMessage}");
                }

                await request.AddAdminLogAsync("重设用户密码", $"用户:{user.UserName}");

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
