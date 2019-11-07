using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    /// <summary>
    /// Administrators
    /// </summary>
    [RoutePrefix("v1/administrators")]
    public class AdministratorsController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsLogout = "actions/logout";
        private const string RouteActionsResetPassword = "actions/resetPassword";
        private const string RouteAdministrator = "{id:int}";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Create()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var administrator = request.GetPostObject<Administrator>("administrator");
                var password = request.GetPostString("password");

                var retVal = await DataProvider.AdministratorDao.InsertAsync(administrator, password);
                if (!retVal.IsValid)
                {
                    return BadRequest(retVal.ErrorMessage);
                }

                return Ok(new
                {
                    Value = administrator
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteAdministrator)]
        public async Task<IHttpActionResult> Update(int id, [FromBody] Administrator administrator)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (administrator == null) return BadRequest("Could not read administrator from body");

                if (!await DataProvider.AdministratorDao.IsExistsAsync(id)) return NotFound();

                administrator.Id = id;

                var retVal = await DataProvider.AdministratorDao.UpdateAsync(administrator);
                if (!retVal.IsValid)
                {
                    return BadRequest(retVal.ErrorMessage);
                }

                return Ok(new
                {
                    Value = administrator
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteAdministrator)]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (!await DataProvider.AdministratorDao.IsExistsAsync(id)) return NotFound();

                var administrator = await DataProvider.AdministratorDao.DeleteAsync(id);

                return Ok(new
                {
                    Value = administrator
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteAdministrator)]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (!await DataProvider.AdministratorDao.IsExistsAsync(id)) return NotFound();

                var administrator = await DataProvider.AdministratorDao.GetByIdAsync(id);

                return Ok(new
                {
                    Value = administrator
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> List()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var administrators = await DataProvider.AdministratorDao.GetAdministratorsAsync(skip, top);
                var count = await DataProvider.AdministratorDao.GetCountAsync();

                return Ok(new PageResponse(administrators, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogin)]
        public async Task<IHttpActionResult> Login()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var isAutoLogin = request.GetPostBool("isAutoLogin");

                Administrator adminInfo;

                var valid = await DataProvider.AdministratorDao.ValidateAsync(account, password, true);

                if (!valid.IsValid)
                {
                    adminInfo = await AdminManager.GetAdminInfoByUserNameAsync(valid.UserName);
                    if (adminInfo != null)
                    {
                        await DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLoginAsync(adminInfo); // 记录最后登录时间、失败次数+1
                    }
                    return BadRequest(valid.ErrorMessage);
                }

                adminInfo = await AdminManager.GetAdminInfoByUserNameAsync(valid.UserName);
                await DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLoginAsync(adminInfo); // 记录最后登录时间、失败次数清零
                var accessToken = request.AdminLogin(adminInfo.UserName, isAutoLogin);
                var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

                var sessionId = StringUtils.Guid();
                var cacheKey = Constants.GetSessionIdCacheKey(adminInfo.Id);
                CacheUtils.Insert(cacheKey, sessionId);

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
                    SessionId = sessionId,
                    IsEnforcePasswordChange = isEnforcePasswordChange
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
                var request = new AuthenticatedRequest();
                var adminInfo = request.IsAdminLoggin ? request.Administrator : null;
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

        [HttpPost, Route(RouteActionsResetPassword)]
        public async Task<IHttpActionResult> ResetPassword()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var isApiAuthorized = request.IsApiAuthenticated && await AccessTokenManager.IsScopeAsync(request.ApiToken, AccessTokenManager.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");

                var valid1 = await DataProvider.AdministratorDao.ValidateAsync(account, password, true);
                if (!valid1.IsValid)
                {
                    return BadRequest(valid1.ErrorMessage);
                }

                var adminInfo = await AdminManager.GetAdminInfoByUserNameAsync(valid1.UserName);

                var valid2 = await DataProvider.AdministratorDao.ChangePasswordAsync(adminInfo, newPassword);
                if (!valid2.IsValid)
                {
                    return BadRequest(valid2.ErrorMessage);
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
