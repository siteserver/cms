using System;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;

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

                var retval = DataProvider.AdministratorDao.ApiInsert(adminInfo, out var errorMessage);
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

                var retval = DataProvider.AdministratorDao.ApiUpdate(id, adminInfo, out var errorMessage);
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
                var count = DataProvider.AdministratorDao.ApiGetCount();

                return Ok(new OResponse(oRequest, administrators) { Count = count });
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

                AdministratorInfo adminInfo;

                if (!DataProvider.AdministratorDao.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                    if (adminInfo != null)
                    {
                        DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo); // 记录最后登录时间、失败次数+1
                    }
                    return BadRequest(errorMessage);
                }

                adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(adminInfo); // 记录最后登录时间、失败次数清零
                var accessToken = request.AdminLogin(adminInfo.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(RequestImpl.AccessTokenExpireDays);

                return Ok(new
                {
                    Value = adminInfo,
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
        public IHttpActionResult ResetPassword()
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeAdministrators);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var account = oRequest.GetPostString("account");
                var password = oRequest.GetPostString("password");
                var newPassword = oRequest.GetPostString("newPassword");

                if (!DataProvider.AdministratorDao.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var adminInfo = AdminManager.GetAdminInfoByUserName(userName);

                if (!DataProvider.AdministratorDao.ChangePassword(adminInfo, newPassword, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new OResponse(adminInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
