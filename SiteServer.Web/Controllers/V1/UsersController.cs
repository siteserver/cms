using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("api")]
    public class UsersController : ApiController
    {
        [HttpGet, Route(ApiUsersRoute.UsersRoute)]
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

        [HttpGet, Route(ApiUsersRoute.UserRoute)]
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

        [HttpPut, Route(ApiUsersRoute.UserRoute)]
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

        [HttpPost, Route(ApiUsersRoute.UsersRoute)]
        public IHttpActionResult PostRegister([FromBody] UserInfo userInfo)
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

        [HttpPost, Route(ApiUsersRoute.ActionsLoginRoute)]
        public IHttpActionResult ActionsLogin([FromBody] ApiUsersRoute.ActionsLoginBody body)
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
    }
}
