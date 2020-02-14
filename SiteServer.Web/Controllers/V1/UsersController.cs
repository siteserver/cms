using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/users")]
    public class UsersController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsLogout = "actions/logout";
        private const string RouteUser = "{id:int}";
        private const string RouteUserAvatar = "{id:int}/avatar";
        private const string RouteUserLogs = "{id:int}/logs";
        private const string RouteUserResetPassword = "{id:int}/actions/resetPassword";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Create()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var user = new User();
                var dict = request.GetPostObject<Dictionary<string, object>>();
                foreach (var o in dict)
                {
                    user.Set(o.Key, o.Value);
                }

                var config = await DataProvider.ConfigRepository.GetAsync();

                if (!config.IsUserRegistrationGroup)
                {
                    user.GroupId = 0;
                }
                var password = request.GetPostString("password");

                var valid = await DataProvider.UserRepository.InsertAsync(user, password, PageUtils.GetIpAddress());
                if (valid.UserId == 0)
                {
                    return BadRequest(valid.ErrorMessage);
                }

                return Ok(new
                {
                    Value = await DataProvider.UserRepository.GetByUserIdAsync(valid.UserId)
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteUser)]
        public async Task<IHttpActionResult> Update(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var body = request.GetPostObject<Dictionary<string, object>>();

                if (body == null) return BadRequest("Could not read user from body");

                var user = await DataProvider.UserRepository.GetByUserIdAsync(id);
                if (user == null) return NotFound();

                var valid = await DataProvider.UserRepository.UpdateAsync(user, body);
                if (valid.User == null)
                {
                    return BadRequest(valid.ErrorMessage);
                }

                return Ok(new
                {
                    Value = valid.User
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteUser)]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                request.UserLogout();
                var user = await DataProvider.UserRepository.DeleteAsync(id);

                return Ok(new
                {
                    Value = user
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUser)]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                if (!await DataProvider.UserRepository.IsExistsAsync(id)) return NotFound();

                var user = await DataProvider.UserRepository.GetByUserIdAsync(id);

                return Ok(new
                {
                    Value = user
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUserAvatar)]
        public async Task<IHttpActionResult> GetAvatar(int id)
        {
            var user = await DataProvider.UserRepository.GetByUserIdAsync(id);

            var avatarUrl = !string.IsNullOrEmpty(user?.AvatarUrl) ? user.AvatarUrl : DataProvider.UserRepository.DefaultAvatarUrl;
            avatarUrl = PageUtils.AddProtocolToUrl(avatarUrl);

            return Ok(new
            {
                Value = avatarUrl
            });
        }

        [HttpPost, Route(RouteUserAvatar)]
        public async Task<IHttpActionResult> UploadAvatar(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var user = await DataProvider.UserRepository.GetByUserIdAsync(id);
                if (user == null) return NotFound();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = DataProvider.UserRepository.GetUserUploadFileName(postFile.FileName);
                    var filePath = DataProvider.UserRepository.GetUserUploadPath(user.Id, fileName);
                    
                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    postFile.SaveAs(filePath);

                    user.AvatarUrl = DataProvider.UserRepository.GetUserUploadUrl(user.Id, fileName);

                    await DataProvider.UserRepository.UpdateAsync(user);
                }

                return Ok(new
                {
                    Value = user
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> List()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await 
                             DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var users = await DataProvider.UserRepository.GetUsersAsync( ETriState.All, 0, 0, null, null, skip, top);
                var count = await DataProvider.UserRepository.GetCountAsync();

                return Ok(new PageResponse(users, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogin)]
        public async Task<IHttpActionResult> Login()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var isAutoLogin = request.GetPostBool("isAutoLogin");

                var valid = await DataProvider.UserRepository.ValidateAsync(account, password, true);
                if (valid.User == null)
                {
                    return BadRequest(valid.ErrorMessage);
                }

                var accessToken = await request.UserLoginAsync(valid.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

                return Ok(new
                {
                    Value = valid.User,
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogout)]
        public async Task<IHttpActionResult> Logout()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var user = request.IsUserLoggin ? request.User : null;
                request.UserLogout();

                return Ok(new
                {
                    Value = user
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUserLogs)]
        public async Task<IHttpActionResult> CreateLog(int id, [FromBody] UserLog log)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var user = await DataProvider.UserRepository.GetByUserIdAsync(id);
                if (user == null) return NotFound();

                var retVal = await DataProvider.UserLogRepository.InsertAsync(user.Id, log);

                return Ok(new
                {
                    Value = retVal
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUserLogs)]
        public async Task<IHttpActionResult> GetLogs(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var user = await DataProvider.UserRepository.GetByUserIdAsync(id);
                if (user == null) return NotFound();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var logs = await DataProvider.UserLogRepository.GetLogsAsync(user.Id, skip, top);

                return Ok(new PageResponse(logs, top, skip, request.HttpRequest.Url.AbsoluteUri)
                    {Count = await DataProvider.UserRepository.GetCountAsync()});
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUserResetPassword)]
        public async Task<IHttpActionResult> ResetPassword(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isAuth = request.IsApiAuthenticated && await
                                 DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             await request.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var user = await DataProvider.UserRepository.GetByUserIdAsync(id);
                if (user == null) return NotFound();

                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");

                if (!DataProvider.UserRepository.CheckPassword(password, false, user.Password, user.PasswordFormat, user.PasswordSalt))
                {
                    return BadRequest("原密码不正确，请重新输入");
                }

                var valid = await DataProvider.UserRepository.ChangePasswordAsync(user.Id, newPassword);
                if (!valid.IsValid)
                {
                    return BadRequest(valid.ErrorMessage);
                }
                
                return Ok(new
                {
                    Value = user
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
