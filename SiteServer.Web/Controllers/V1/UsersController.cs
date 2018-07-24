using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
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
        private const string RouteActionsResetPassword = "v1/users/actions/resetPassword";

        private const string RouteUser = "v1/users/{id:int}";
        private const string RouteUserAvatar = "v1/users/{id:int}/avatar";
        private const string RouteUserLogs = "v1/users/{id:int}/logs";

        [HttpPost, Route(Route)]
        public IHttpActionResult Create([FromBody] UserInfoCreateUpdate userInfo)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                string errorMessage;
                var retval = DataProvider.UserDao.ApiInsert(userInfo, PageUtils.GetIpAddress(), out errorMessage);
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

        [HttpPut, Route(RouteUser)]
        public IHttpActionResult Update(int id, [FromBody] UserInfoCreateUpdate userInfo)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (userInfo == null) return BadRequest("Could not read user from body");

                if (!DataProvider.UserDao.ApiIsExists(id)) return NotFound();

                string errorMessage;
                var retval = DataProvider.UserDao.ApiUpdate(id, userInfo, out errorMessage);
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
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (!DataProvider.UserDao.ApiIsExists(id)) return NotFound();

                var userInfo = DataProvider.UserDao.ApiDelete(id);

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
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (!DataProvider.UserDao.ApiIsExists(id)) return NotFound();

                var user = DataProvider.UserDao.ApiGetUser(id);

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
            var userInfo = DataProvider.UserDao.ApiGetUser(id);

            var avatarUrl = !string.IsNullOrEmpty(userInfo?.AvatarUrl) ? userInfo.AvatarUrl : PageUtils.GetUserFilesUrl(string.Empty, "default_avatar.png");
            avatarUrl = PageUtils.AddProtocolToUrl(avatarUrl);

            return Ok(new OResponse(avatarUrl));
        }

        [HttpPost, Route(RouteUserAvatar)]
        public IHttpActionResult UploadAvatar(int id)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (!DataProvider.UserDao.ApiIsExists(id)) return NotFound();

                var userInfo = DataProvider.UserDao.ApiGetUser(id);

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var directoryPath = PathUtils.GetUserUploadDirectoryPath(userInfo.UserName);
                    var fileName = PathUtils.GetUserUploadFileName(postFile.FileName);
                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(PathUtils.Combine(directoryPath, fileName));

                    userInfo.AvatarUrl = PageUtils.AddProtocolToUrl(PageUtils.GetUserFilesUrl(userInfo.UserName, fileName));
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
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var users = DataProvider.UserDao.ApiGetUsers(oRequest.Skip, oRequest.Top);
                var oResponse = new OResponse(users);

                var count = DataProvider.UserDao.ApiGetCount();
                if (oRequest.Count)
                {
                    oResponse.Count = count;
                }

                if (oRequest.Top + oRequest.Skip < count)
                {
                    oResponse.Next =
                        PageUtils.AddQueryString(
                            PageUtils.RemoveQueryString(oRequest.RawUrl, new List<string> {"top", "skip"}),
                            new NameValueCollection
                            {
                                {"top", oRequest.Top.ToString()},
                                {"skip", (oRequest.Top + oRequest.Skip).ToString()}
                            });
                }

                return Ok(oResponse);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult Login([FromBody] ActionsLoginBody body)
        {
            try
            {
                var request = new AuthRequest();

                string userName;
                string errorMessage;
                if (!DataProvider.UserDao.Validate(body.Account, body.Password, true, out userName, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var userInfo = DataProvider.UserDao.GetUserInfoByUserName(userName);

                var accessToken = request.UserLogin(userName, body.IsAutoLogin);

                return Ok(new
                {
                    Value = userInfo,
                    AccessToken = accessToken
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
                var request = new AuthRequest();
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

        [HttpPost, Route(RouteActionsResetPassword)]
        public IHttpActionResult ResetPassword([FromBody] ActionsResetPasswordBody body)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeUsers);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                string userName;
                string errorMessage;
                if (!DataProvider.UserDao.Validate(body.Account, body.Password, true, out userName, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                if (!DataProvider.UserDao.ChangePassword(userName, body.NewPassword, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var userInfo = DataProvider.UserDao.GetUserInfoByUserName(userName);

                return Ok(new OResponse(userInfo));
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

                if (!DataProvider.UserDao.ApiIsExists(id)) return NotFound();

                var userName = DataProvider.UserDao.GetUserName(id);

                var retval = DataProvider.UserLogDao.ApiInsert(userName, logInfo);

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

                var userName = DataProvider.UserDao.GetUserName(id);
                if (string.IsNullOrEmpty(userName)) return NotFound();

                var logs = DataProvider.UserLogDao.ApiGetLogs(userName, oRequest.Skip, oRequest.Top);
                var oResponse = new OResponse(logs);

                var count = DataProvider.UserDao.ApiGetCount();
                if (oRequest.Count)
                {
                    oResponse.Count = count;
                }

                if (oRequest.Top + oRequest.Skip < count)
                {
                    oResponse.Next =
                        PageUtils.AddQueryString(
                            PageUtils.RemoveQueryString(oRequest.RawUrl, new List<string> { "top", "skip" }),
                            new NameValueCollection
                            {
                                {"top", oRequest.Top.ToString()},
                                {"skip", (oRequest.Top + oRequest.Skip).ToString()}
                            });
                }

                return Ok(oResponse);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        public class ActionsLoginBody
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public bool IsAutoLogin { get; set; }
        }

        public class ActionsResetPasswordBody
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
