using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("api")]
    public class UsersController : ApiController
    {
        private const string Route = "v1/users";
        private const string RouteActionsLogin = "v1/users/actions/login";
        private const string RouteActionsLogout = "v1/users/actions/logout";

        private const string RouteUser = "v1/users/{id:int}";
        private const string RouteUserAvatar = "v1/users/{id:int}/avatar";
        private const string RouteUserLogs = "v1/users/{id:int}/logs";
        private const string RouteUserResetPassword = "v1/users/{id:int}/actions/resetPassword";

        [HttpPost, Route(Route)]
        public IHttpActionResult Create()
        {
            try
            {
                var request = new RequestImpl(AccessTokenManager.ScopeUsers);
                var userInfo = new UserInfo(request.GetPostObject<Dictionary<string, object>>());
                var password = request.GetPostString("password");

                var userId = DataProvider.UserDao.Insert(userInfo, password, PageUtils.GetIpAddress(), out var errorMessage);
                if (userId == 0)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new OResponse(UserManager.GetUserInfoByUserId(userId)));
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
                var request = new RequestImpl(AccessTokenManager.ScopeUsers);
                if (!request.IsUserAuthorized(id)) return Unauthorized();

                var body = request.GetPostObject<Dictionary<string, object>>();

                if (body == null) return BadRequest("Could not read user from body");

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retval = DataProvider.UserDao.Update(userInfo, body, out var errorMessage);
                if (retval == null)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new OResponse(retval));
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
                var request = new RequestImpl(AccessTokenManager.ScopeUsers);
                if (!request.IsUserAuthorized(id)) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                request.UserLogout();
                DataProvider.UserDao.Delete(userInfo);

                return Ok(new OResponse(userInfo));
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
                var request = new RequestImpl(AccessTokenManager.ScopeUsers);
                if (!request.IsUserAuthorized(id)) return Unauthorized();

                if (!DataProvider.UserDao.IsExists(id)) return NotFound();

                var user = UserManager.GetUserInfoByUserId(id);

                return Ok(new OResponse(user));
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

            var avatarUrl = !string.IsNullOrEmpty(userInfo?.AvatarUrl) ? userInfo.AvatarUrl : PageUtils.GetUserFilesUrl(string.Empty, "default_avatar.png");
            avatarUrl = PageUtils.AddProtocolToUrl(avatarUrl);

            return Ok(new OResponse(avatarUrl));
        }

        [HttpPost, Route(RouteUserAvatar)]
        public IHttpActionResult UploadAvatar(int id)
        {
            try
            {
                var request = new RequestImpl(AccessTokenManager.ScopeUsers);
                if (!request.IsUserAuthorized(id)) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = PathUtils.GetUserUploadFileName(postFile.FileName);
                    var filePath = PathUtils.GetUserFilesPath(userInfo.UserName, fileName);
                    
                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                    postFile.SaveAs(filePath);

                    userInfo.AvatarUrl = PageUtils.AddProtocolToUrl(PageUtils.GetUserFilesUrl(userInfo.UserName, fileName));

                    DataProvider.UserDao.Update(userInfo);
                }

                var oResponse = new OResponse(userInfo);

                return Ok(oResponse);
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
                var request = new ORequest(AccessTokenManager.ScopeUsers);
                if (!request.IsApiAuthorized) return Unauthorized();

                var users = DataProvider.UserDao.GetUsers(request.Skip, request.Top);
                var count = DataProvider.UserDao.GetCount();

                return Ok(new OResponse(request, users) { Count = count });
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
                var request = new RequestImpl();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var isAutoLogin = request.GetPostBool("isAutoLogin");

                var userInfo = UserManager.GetUserInfoByAccount(account);
                if (userInfo == null)
                {
                    return BadRequest("帐号或密码错误");
                }

                if (!DataProvider.UserDao.Validate(account, password, true, out var _, out var errorMessage))
                {
                    DataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userInfo);
                    return BadRequest(errorMessage);
                }

                var accessToken = request.UserLogin(userInfo.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(RequestImpl.AccessTokenExpireDays);

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
                var request = new RequestImpl();
                var response = new OResponse(request.IsUserLoggin ? request.UserInfo : null);
                request.UserLogout();
                return Ok(response);
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
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var retval = DataProvider.UserLogDao.ApiInsert(userInfo.UserName, logInfo);

                return Ok(new OResponse(retval));
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
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var userInfo = UserManager.GetUserInfoByUserId(id);
                if (userInfo == null) return NotFound();

                var logs = DataProvider.UserLogDao.ApiGetLogs(userInfo.UserName, oRequest.Skip, oRequest.Top);

                return Ok(new OResponse(oRequest, logs) { Count = DataProvider.UserDao.GetCount() });
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
                var request = new RequestImpl(AccessTokenManager.ScopeUsers);
                if (!request.IsUserAuthorized(id)) return Unauthorized();

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

                return Ok(new OResponse(userInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
