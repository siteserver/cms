using System;
using System.Collections.Generic;
using System.Collections.Specialized;using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("api")]
    public class AdministratorsController : ApiController
    {
        private const string Route = "v1/administrators";
        private const string RouteActionsLogin = "v1/administrators/actions/login";
        private const string RouteActionsLogout = "v1/administrators/actions/logout";
        private const string RouteActionsResetPassword = "v1/administrators/actions/resetPassword";

        private const string RouteAdministrator = "v1/administrators/{id:int}";

        [HttpPost, Route(Route)]
        public IHttpActionResult Create([FromBody] AdministratorInfoCreateUpdate adminInfo)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                string errorMessage;
                var retval = DataProvider.AdministratorDao.ApiInsert(adminInfo, out errorMessage);
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

        [HttpPut, Route(RouteAdministrator)]
        public IHttpActionResult Update(int id, [FromBody] AdministratorInfoCreateUpdate adminInfo)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (adminInfo == null) return BadRequest("Could not read administrator from body");

                if (!DataProvider.AdministratorDao.ApiIsExists(id)) return NotFound();

                string errorMessage;
                var retval = DataProvider.AdministratorDao.ApiUpdate(id, adminInfo, out errorMessage);
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

        [HttpDelete, Route(RouteAdministrator)]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (!DataProvider.AdministratorDao.ApiIsExists(id)) return NotFound();

                var adminInfo = DataProvider.AdministratorDao.ApiDelete(id);

                return Ok(new OResponse(adminInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteAdministrator)]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (!DataProvider.AdministratorDao.ApiIsExists(id)) return NotFound();

                var adminInfo = DataProvider.AdministratorDao.ApiGetAdministrator(id);

                return Ok(new OResponse(adminInfo));
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
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var administrators = DataProvider.AdministratorDao.ApiGetAdministrators(oRequest.Skip, oRequest.Top);
                var oResponse = new OResponse(administrators);

                var count = DataProvider.AdministratorDao.ApiGetCount();
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
                if (!DataProvider.AdministratorDao.Validate(body.Account, body.Password, true, out userName, out errorMessage))
                {
                    DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName); // 记录最后登录时间、失败次数+1
                    return BadRequest(errorMessage);
                }

                var adminInfo = DataProvider.AdministratorDao.GetByUserName(userName);

                DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName); // 记录最后登录时间、失败次数清零
                var accessToken = request.AdminLogin(userName, body.IsAutoLogin);

                return Ok(new
                {
                    Value = adminInfo,
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
                var response = new OResponse(request.IsAdminLoggin ? request.AdminInfo : null);
                request.AdminLogout();
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
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                string userName;
                string errorMessage;
                if (!DataProvider.AdministratorDao.Validate(body.Account, body.Password, true, out userName, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                if (!DataProvider.AdministratorDao.ChangePassword(userName, body.NewPassword, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var adminInfo = DataProvider.AdministratorDao.GetByUserName(userName);

                return Ok(new OResponse(adminInfo));
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
