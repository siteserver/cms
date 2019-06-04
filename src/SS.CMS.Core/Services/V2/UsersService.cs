using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Services.V2
{
    public class UsersService : ServiceBase
    {
        private const string Route = "users";
        private const string RouteActionsLogin = "users/actions/login";
        private const string RouteActionsLogout = "users/actions/logout";
        private const string RouteUser = "users/{id:int}";
        private const string RouteUserAvatar = "users/{id:int}/avatar";
        private const string RouteUserLogs = "users/{id:int}/logs";
        private const string RouteUserResetPassword = "users/{id:int}/actions/resetPassword";

        public ResponseResult<object> Create(IRequest request, IResponse response)
        {
            if (!request.TryGetPost<Dictionary<string, object>>(out var dict))
            {
                return BadRequest("参数不正确");
            }

            var userInfo = new UserInfo();
            foreach (var o in dict)
            {
                userInfo.Set(o.Key, o.Value);
            }

            if (!ConfigManager.Instance.IsUserRegistrationGroup)
            {
                userInfo.GroupId = 0;
            }
            var password = request.GetPostString("password");

            var userId = DataProvider.UserDao.Insert(userInfo, password, request.IpAddress, out var errorMessage);
            if (userId == 0)
            {
                return BadRequest(errorMessage);
            }

            return Ok(new
            {
                Value = UserManager.GetUserInfoByUserId(userId)
            });
        }

        public ResponseResult<object> Update(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                         request.IsUserLoggin &&
                         request.UserId == id ||
                         request.IsAdminLoggin &&
                         request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
            if (!isAuth) return Unauthorized();

            if (!request.TryGetPost<Dictionary<string, object>>(out var body))
            {
                return BadRequest("Could not read user from body");
            }

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

        public ResponseResult<object> Delete(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                         request.IsUserLoggin &&
                         request.UserId == id ||
                         request.IsAdminLoggin &&
                         request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
            if (!isAuth) return Unauthorized();

            var userInfo = UserManager.GetUserInfoByUserId(id);
            if (userInfo == null) return NotFound();

            response.UserLogout();
            DataProvider.UserDao.Delete(userInfo);

            return Ok(new
            {
                Value = userInfo
            });
        }

        public ResponseResult<object> Get(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                         request.IsUserLoggin &&
                         request.UserId == id ||
                         request.IsAdminLoggin &&
                         request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
            if (!isAuth) return Unauthorized();

            if (!DataProvider.UserDao.IsExists(id)) return NotFound();

            var user = UserManager.GetUserInfoByUserId(id);

            return Ok(new
            {
                Value = user
            });
        }

        public ResponseResult<object> GetAvatar(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var userInfo = UserManager.GetUserInfoByUserId(id);

            var avatarUrl = !string.IsNullOrEmpty(userInfo?.AvatarUrl) ? userInfo.AvatarUrl : UserManager.DefaultAvatarUrl;
            avatarUrl = PageUtilsEx.AddProtocolToUrl(avatarUrl);

            return Ok(new
            {
                Value = avatarUrl
            });
        }

        public ResponseResult<object> UploadAvatar(IRequest request, IResponse response)
        {
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                         request.IsUserLoggin &&
                         request.UserId == id ||
                         request.IsAdminLoggin &&
                         request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
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

        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var request = GetRequest();
                var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var users = DataProvider.UserDao.GetUsers(skip, top);
                var count = DataProvider.UserDao.GetCount();

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
                var request = GetRequest();
                var response = GetResponse();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var isAutoLogin = request.GetPostBool("isAutoLogin");

                var userInfo = DataProvider.UserDao.Validate(account, password, true, out var _, out var errorMessage);
                if (userInfo == null)
                {
                    return BadRequest(errorMessage);
                }

                var accessToken = response.UserLogin(userInfo.UserName, isAutoLogin);
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

        [HttpPost, Route(RouteActionsLogout)]
        public IHttpActionResult Logout()
        {
            try
            {
                var request = GetRequest();
                var response = GetResponse();

                var userInfo = UserManager.GetUserInfoByUserId(request.UserId);
                response.UserLogout();

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
                var request = GetRequest();
                var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retval = DataProvider.UserLogDao.Insert(userInfo.UserName, logInfo);

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
                var request = GetRequest();
                var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
                if (!isAuth) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var logs = DataProvider.UserLogDao.ApiGetLogs(userInfo.UserName, skip, top);

                return Ok(new PageResponse(logs, top, skip, Request.RequestUri.AbsoluteUri) { Count = DataProvider.UserDao.GetCount() });
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
                var request = GetRequest();
                var isAuth = AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeUsers) ||
                             request.IsUserLoggin &&
                             request.UserId == id ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasSystemPermissions(ConfigManager.SettingsPermissions.User);
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
    }
}
