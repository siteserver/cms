using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        [HttpGet, Route(ApiUsersRoute.Route)]
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

        [HttpPost, Route(ApiUsersRoute.Route)]
        public IHttpActionResult PostRegister(UserInfo userInfo)
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
        public IHttpActionResult ActionsLogin(ApiUsersRoute.ActionsLoginBody body)
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
