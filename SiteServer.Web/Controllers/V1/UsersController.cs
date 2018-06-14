using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    public class ActionsLoginBody
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

    public class ActionsResetPasswordBody
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }

    [RoutePrefix("api")]
    public class UsersController : ApiController
    {
        private const string RouteUsers = "v1/users";
        private const string RouteUser = "v1/users/{id}";
        private const string RouteUserAvatar = "v1/users/{id}/avatar";
        private const string RouteActionsLogin = "v1/users/actions/login";
        private const string RouteActionsResetPassword = "v1/users/actions/resetPassword";

        [HttpGet, Route(RouteUsers)]
        public IHttpActionResult GetUsers()
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Unauthorized();
                }

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

        [HttpGet, Route(RouteUser)]
        public IHttpActionResult GetUser(int id)
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Unauthorized();
                }

                var user = DataProvider.UserDao.ApiGetUser(id);
                var oResponse = new OResponse(user);

                return Ok(oResponse);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteUser)]
        public HttpResponseMessage UpdateUser(int id, [FromBody] UserInfo userInfo)
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                }

                if (userInfo == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read user from body");

                if (!DataProvider.UserDao.ApiIsExists(id)) return Request.CreateResponse(HttpStatusCode.NoContent, "User is not found");

                DataProvider.UserDao.ApiUpdateUser(id, userInfo);

                var oResponse = new OResponse(userInfo);

                return Request.CreateResponse(oResponse);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route(RouteUsers)]
        public IHttpActionResult Register([FromBody] UserInfo userInfo)
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Unauthorized();
                }

                string errorMessage;
                if (!DataProvider.UserDao.ApiInsert(userInfo, PageUtils.GetIpAddress(), out errorMessage))
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

        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult ActionsLogin([FromBody] ActionsLoginBody body)
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Unauthorized();
                }

                string userName;
                string errorMessage;
                if (!DataProvider.UserDao.Validate(body.Account, body.Password, true, out userName, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var userInfo = DataProvider.UserDao.GetUserInfoByUserName(userName);

                oRequest.AuthRequest.UserLogin(userName);

                return Ok(new OResponse(userInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsResetPassword)]
        public IHttpActionResult ActionsResetPassword([FromBody] ActionsResetPasswordBody body)
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Unauthorized();
                }

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

        [HttpGet, Route(RouteUserAvatar)]
        public string GetAvatar(int id)
        {
            var userInfo = DataProvider.UserDao.ApiGetUser(id);

            return !string.IsNullOrEmpty(userInfo?.AvatarUrl) ? userInfo.AvatarUrl : string.Empty;
        }

        [HttpPost, Route(RouteUserAvatar)]
        public IHttpActionResult UploadAvatar(HttpPostedFileBase uploadedFile)
        {
            try
            {
                var oRequest = new ORequest();

                if (!oRequest.IsAuthorized(AccessTokenManager.ScopeUsers))
                {
                    return Unauthorized();
                }

                var users = DataProvider.UserDao.ApiGetUsers(oRequest.Skip, oRequest.Top);
                var oResponse = new OResponse(users);

                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();

                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }

                    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
