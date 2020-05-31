using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

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

        [OpenApiOperation("新增用户 API", "https://sscms.com/docs/v6/api/guide/users/create.html")]
        [HttpPost, Route(Route)]
        public IHttpActionResult Create()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var userInfo = new UserInfo(request.GetPostObject<Dictionary<string, object>>());
                if (!ConfigManager.SystemConfigInfo.IsUserRegistrationGroup)
                {
                    userInfo.GroupId = 0;
                }
                var password = request.GetPostString("password");

                var userId = DataProvider.UserDao.Insert(userInfo, password, PageUtils.GetIpAddress(), out var errorMessage);
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

        [OpenApiOperation("修改用户 API", "https://sscms.com/docs/v6/api/guide/users/update.html")]
        [HttpPut, Route(RouteUser)]
        public IHttpActionResult Update(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var body = request.GetPostObject<Dictionary<string, object>>();

                if (body == null) return BadRequest("Could not read user from body");

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retVal = DataProvider.UserDao.Update(userInfo, body, out var errorMessage);
                if (retVal == null)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = retVal
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("删除用户 API", "https://sscms.com/docs/v6/api/guide/users/delete.html")]
        [HttpDelete, Route(RouteUser)]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                request.UserLogout();
                DataProvider.UserDao.Delete(userInfo);

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

        [OpenApiOperation("获取用户 API", "https://sscms.com/docs/v6/api/guide/users/get.html")]
        [HttpGet, Route(RouteUser)]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                if (!DataProvider.UserDao.IsExists(id)) return NotFound();

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

        [OpenApiOperation("获取用户头像 API", "https://sscms.com/docs/v6/api/guide/users/getAvatar.html")]
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

        [OpenApiOperation("上传用户头像 API", "https://sscms.com/docs/v6/api/guide/users/updateAvatar.html")]
        [HttpPost, Route(RouteUserAvatar)]
        public IHttpActionResult UploadAvatar(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
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

                    DataProvider.UserDao.Update(userInfo);
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

        [OpenApiOperation("获取用户列表 API", "https://sscms.com/docs/v6/api/guide/users/list.html")]
        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var users = DataProvider.UserDao.GetUsers(skip, top);
                var count = DataProvider.UserDao.GetCount();

                return Ok(new PageResponse(users, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("用户登录 API", "https://sscms.com/docs/v6/api/guide/users/login.html")]
        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult Login()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var isAutoLogin = request.GetPostBool("isAutoLogin");

                var userInfo = DataProvider.UserDao.Validate(account, password, true, out var _, out var errorMessage);
                if (userInfo == null)
                {
                    return BadRequest(errorMessage);
                }

                var accessToken = request.UserLogin(userInfo.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

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

        [OpenApiOperation("用户退出登录 API", "https://sscms.com/docs/v6/api/guide/users/logout.html")]
        [HttpPost, Route(RouteActionsLogout)]
        public IHttpActionResult Logout()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var userInfo = request.IsUserLoggin ? request.UserInfo : null;
                request.UserLogout();

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

        [OpenApiOperation("修改用户密码 API", "https://sscms.com/docs/v6/api/guide/users/resetPassword.html")]
        [HttpPost, Route(RouteUserResetPassword)]
        public IHttpActionResult ResetPassword(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");

                if (!DataProvider.UserDao.CheckPassword(password, false, userInfo.Password, EPasswordFormatUtils.GetEnumType(userInfo.PasswordFormat), userInfo.PasswordSalt))
                {
                    return BadRequest("原密码不正确，请重新输入");
                }

                if (!DataProvider.UserDao.ChangePassword(userInfo.UserName, newPassword, out string errorMessage))
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

        [OpenApiOperation("新增用户操作日志 API", "https://sscms.com/docs/v6/api/guide/users/createLog.html")]
        [HttpPost, Route(RouteUserLogs)]
        public IHttpActionResult CreateLog(int id, [FromBody] UserLogInfo logInfo)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retVal = DataProvider.UserLogDao.ApiInsert(userInfo.UserName, logInfo);

                return Ok(new
                {
                    Value = retVal
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("获取用户操作日志 API", "https://sscms.com/docs/v6/api/guide/users/getLogs.html")]
        [HttpGet, Route(RouteUserLogs)]
        public IHttpActionResult GetLogs(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUser);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var logs = DataProvider.UserLogDao.ApiGetLogs(userInfo.UserName, skip, top);

                return Ok(new PageResponse(logs, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = DataProvider.UserDao.GetCount() });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
