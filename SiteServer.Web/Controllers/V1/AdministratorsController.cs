using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/administrators")]
    public class AdministratorsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsLogout = "actions/logout";
        private const string RouteActionsResetPassword = "actions/resetPassword";
        private const string RouteAdministrator = "{id:int}";

        [OpenApiOperation("新增管理员 API", "https://sscms.com/docs/v6/api/guide/administrators/create.html")]
        [HttpPost, Route(Route)]
        public IHttpActionResult Create([FromBody] AdministratorInfoCreateUpdate adminInfo)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var retVal = DataProvider.AdministratorDao.ApiInsert(adminInfo, out var errorMessage);
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

        [OpenApiOperation("修改管理员 API", "https://sscms.com/docs/v6/api/guide/administrators/update.html")]
        [HttpPut, Route(RouteAdministrator)]
        public IHttpActionResult Update(int id, [FromBody] AdministratorInfoCreateUpdate adminInfo)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (adminInfo == null) return BadRequest("Could not read administrator from body");

                if (!DataProvider.AdministratorDao.ApiIsExists(id)) return NotFound();

                var retVal = DataProvider.AdministratorDao.ApiUpdate(id, adminInfo, out var errorMessage);
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

        [OpenApiOperation("删除管理员 API", "https://sscms.com/docs/v6/api/guide/administrators/delete.html")]
        [HttpDelete, Route(RouteAdministrator)]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (!DataProvider.AdministratorDao.ApiIsExists(id)) return NotFound();

                var adminInfo = DataProvider.AdministratorDao.ApiDelete(id);

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("获取管理员 API", "https://sscms.com/docs/v6/api/guide/administrators/get.html")]
        [HttpGet, Route(RouteAdministrator)]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (!DataProvider.AdministratorDao.ApiIsExists(id)) return NotFound();

                var adminInfo = DataProvider.AdministratorDao.ApiGetAdministrator(id);

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("获取管理员列表 API", "https://sscms.com/docs/v6/api/guide/administrators/list.html")]
        [HttpGet, Route(Route)]
        public IHttpActionResult List()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var administrators = DataProvider.AdministratorDao.ApiGetAdministrators(skip, top);
                var count = DataProvider.AdministratorDao.ApiGetCount();

                return Ok(new PageResponse(administrators, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("管理员登录 API", "https://sscms.com/docs/v6/api/guide/administrators/login.html")]
        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult Login()
        {
            try
            {
                var request = new AuthenticatedRequest();

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
                var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

                var isEnforcePasswordChange = false;
                if (ConfigManager.SystemConfigInfo.IsAdminEnforcePasswordChange)
                {
                    if (adminInfo.LastChangePasswordDate == null)
                    {
                        isEnforcePasswordChange = true;
                    }
                    else
                    {
                        var ts = new TimeSpan(DateTime.Now.Ticks - adminInfo.LastChangePasswordDate.Value.Ticks);
                        if (ts.TotalDays > ConfigManager.SystemConfigInfo.AdminEnforcePasswordChangeDays)
                        {
                            isEnforcePasswordChange = true;
                        }
                    }
                }

                return Ok(new
                {
                    Value = adminInfo,
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt,
                    IsEnforcePasswordChange = isEnforcePasswordChange
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("管理员退出登录 API", "https://sscms.com/docs/v6/api/guide/administrators/logout.html")]
        [HttpPost, Route(RouteActionsLogout)]
        public IHttpActionResult Logout()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var adminInfo = request.IsAdminLoggin ? request.AdminInfo : null;
                request.AdminLogout();

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("修改管理员密码 API", "https://sscms.com/docs/v6/api/guide/administrators/resetPassword.html")]
        [HttpPost, Route(RouteActionsResetPassword)]
        public IHttpActionResult ResetPassword()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");

                if (!DataProvider.AdministratorDao.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                var adminInfo = AdminManager.GetAdminInfoByUserName(userName);

                if (!DataProvider.AdministratorDao.ChangePassword(adminInfo, newPassword, out errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new
                {
                    Value = adminInfo
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
