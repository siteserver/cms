using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.V1;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/users")]
    public class V1UsersController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsLogout = "actions/logout";
        private const string RouteUser = "{id:int}";
        private const string RouteUserAvatar = "{id:int}/avatar";
        private const string RouteUserLogs = "{id:int}/logs";
        private const string RouteUserResetPassword = "{id:int}/actions/resetPassword";

        [HttpPost, Route(Route)]
        public IHttpActionResult Create()
        {
            try
            {
                var rest = new Rest(Request);
                var dict = rest.GetPostObject<Dictionary<string, object>>();

                var userInfo = new UserInfo();
                foreach (var o in dict)
                {
                    userInfo.Set(o.Key, o.Value);
                }

                if (!ConfigManager.Instance.IsUserRegistrationGroup)
                {
                    userInfo.GroupId = 0;
                }
                var password = rest.GetPostString("password");

                var userId = DataProvider.User.Insert(userInfo, password, PageUtils.GetIpAddress(), out var errorMessage);
                if (userId == 0)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = UserManager.GetUserInfoByUserId(userId)
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteUser)]
        public IHttpActionResult Update(int id)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var body = rest.GetPostObject<Dictionary<string, object>>();

                if (body == null) return BadRequest("Could not read user from body");

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retval = DataProvider.User.Update(userInfo, body, out var errorMessage);
                if (retval == null)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = retval
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteUser)]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                rest.UserLogout();
                DataProvider.User.Delete(userInfo);

                return Ok(new
                {
                    Value = userInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUser)]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                if (!DataProvider.User.IsExists(id)) return NotFound();

                var user = UserManager.GetUserInfoByUserId(id);

                return Ok(new
                {
                    Value = user
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUserAvatar)]
        public IHttpActionResult GetAvatar(int id)
        {
            var userInfo = UserManager.GetUserInfoByUserId(id);

            var avatarUrl = !string.IsNullOrEmpty(userInfo?.AvatarUrl) ? userInfo.AvatarUrl : UserManager.DefaultAvatarUrl;
            avatarUrl = PageUtils.AddProtocolToUrl(avatarUrl);

            return Ok(new
            {
                Value = avatarUrl
            });
        }

        [HttpPost, Route(RouteUserAvatar)]
        public IHttpActionResult UploadAvatar(int id)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = UserManager.GetUserUploadFileName(postFile.FileName);
                    var filePath = UserManager.GetUserUploadPath(userInfo.Id, fileName);
                    
                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    postFile.SaveAs(filePath);

                    userInfo.AvatarUrl = UserManager.GetUserUploadUrl(userInfo.Id, fileName);

                    DataProvider.User.Update(userInfo);
                }

                return Ok(new
                {
                    Value = userInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var top = rest.GetQueryInt("top", 20);
                var skip = rest.GetQueryInt("skip");

                var users = DataProvider.User.GetUsers(skip, top);
                var count = DataProvider.User.GetCount();

                return Ok(new PageResponse(users, top, skip, Request.RequestUri.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult Login()
        {
            try
            {
                var rest = new Rest(Request);

                var account = rest.GetPostString("account");
                var password = rest.GetPostString("password");
                var isAutoLogin = rest.GetPostBool("isAutoLogin");

                var userInfo = DataProvider.User.Validate(account, password, true, out var _, out var errorMessage);
                if (userInfo == null)
                {
                    return BadRequest(errorMessage);
                }

                var accessToken = rest.UserLogin(userInfo.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(Rest.AccessTokenExpireDays);

                return Ok(new
                {
                    Value = userInfo,
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogout)]
        public IHttpActionResult Logout()
        {
            try
            {
                var rest = new Rest(Request);
                var userInfo = rest.IsUserLoggin ? rest.UserInfo : null;
                rest.UserLogout();

                return Ok(new
                {
                    Value = userInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUserLogs)]
        public IHttpActionResult CreateLog(int id, [FromBody] UserLogInfo logInfo)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retval = DataProvider.UserLog.Insert(userInfo.UserName, logInfo);

                return Ok(new
                {
                    Value = retval
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteUserLogs)]
        public IHttpActionResult GetLogs(int id)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var top = rest.GetQueryInt("top", 20);
                var skip = rest.GetQueryInt("skip");

                var logs = DataProvider.UserLog.ApiGetLogs(userInfo.UserName, skip, top);

                return Ok(new PageResponse(logs, top, skip, Request.RequestUri.AbsoluteUri) { Count = DataProvider.User.GetCount() });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUserResetPassword)]
        public IHttpActionResult ResetPassword(int id)
        {
            try
            {
                var rest = new Rest(Request);
                var isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeUsers) ||
                             rest.IsUserLoggin &&
                             rest.UserId == id ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var password = rest.GetPostString("password");
                var newPassword = rest.GetPostString("newPassword");

                if (!DataProvider.User.CheckPassword(password, false, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
                {
                    return BadRequest("原密码不正确，请重新输入");
                }

                if (!DataProvider.User.ChangePassword(userInfo.UserName, newPassword, out string errorMessage))
                {
                    return BadRequest(errorMessage);
                }
                
                return Ok(new
                {
                    Value = userInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
