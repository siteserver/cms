using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.V1
{
    /// <summary>
    /// Administrators
    /// </summary>
    [RoutePrefix("v1/administrators")]
    public partial class AdministratorsController : ApiController
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var administrator = request.GetPostObject<Administrator>("administrator");
                var password = request.GetPostString("password");

                var retVal = await DataProvider.AdministratorRepository.InsertAsync(administrator, password);
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
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteAdministrator)]
        public async Task<IHttpActionResult> Update(int id, [FromBody] Administrator administrator)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (administrator == null) return BadRequest("Could not read administrator from body");

                if (!await DataProvider.AdministratorRepository.IsExistsAsync(id)) return NotFound();

                administrator.Id = id;

                var retVal = await DataProvider.AdministratorRepository.UpdateAsync(administrator);
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
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteAdministrator)]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (!await DataProvider.AdministratorRepository.IsExistsAsync(id)) return NotFound();

                var administrator = await DataProvider.AdministratorRepository.DeleteAsync(id);

                return Ok(new
                {
                    Value = administrator
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteAdministrator)]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                if (!await DataProvider.AdministratorRepository.IsExistsAsync(id)) return NotFound();

                var administrator = await DataProvider.AdministratorRepository.GetByUserIdAsync(id);

                return Ok(new
                {
                    Value = administrator
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> List()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");

                var administrators = await DataProvider.AdministratorRepository.GetAdministratorsAsync(skip, top);
                var count = await DataProvider.AdministratorRepository.GetCountAsync();

                return Ok(new PageResponse(administrators, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsLogin)]
        public async Task<LoginResult> Login([FromBody] LoginRequest request)
        {
            var context = await AuthenticatedRequest.GetAuthAsync();

            Administrator adminInfo;

            var (isValid, userName, errorMessage) = await DataProvider.AdministratorRepository.ValidateAsync(request.Account, request.Password, true);

            if (!isValid)
            {
                adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
                if (adminInfo != null)
                {
                    await DataProvider.AdministratorRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(adminInfo); // 记录最后登录时间、失败次数+1
                }

                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    errorMessage
                ));
            }

            adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
            await DataProvider.AdministratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(adminInfo); // 记录最后登录时间、失败次数清零
            var accessToken = await context.AdminLoginAsync(adminInfo.UserName, request.IsAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(adminInfo.Id);
            CacheUtils.Insert(cacheKey, sessionId);

            var config = await DataProvider.ConfigRepository.GetAsync();

            var isEnforcePasswordChange = false;
            if (config.IsAdminEnforcePasswordChange)
            {
                if (adminInfo.LastChangePasswordDate == null)
                {
                    isEnforcePasswordChange = true;
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - adminInfo.LastChangePasswordDate.Value.Ticks);
                    if (ts.TotalDays > config.AdminEnforcePasswordChangeDays)
                    {
                        isEnforcePasswordChange = true;
                    }
                }
            }

            return new LoginResult
            {
                Value = adminInfo,
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange
            };
        }

        [HttpPost, Route(RouteActionsLogout)]
        public async Task<IHttpActionResult> Logout()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var adminInfo = request.IsAdminLoggin ? request.Administrator : null;
                request.AdminLogout();

                return Ok(new
                {
                    Value = adminInfo
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsResetPassword)]
        public async Task<IHttpActionResult> ResetPassword()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var isApiAuthorized = request.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeAdministrators);
                if (!isApiAuthorized) return Unauthorized();

                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                var newPassword = request.GetPostString("newPassword");

                var valid1 = await DataProvider.AdministratorRepository.ValidateAsync(account, password, true);
                if (!valid1.IsValid)
                {
                    return BadRequest(valid1.ErrorMessage);
                }

                var adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(valid1.UserName);

                var valid2 = await DataProvider.AdministratorRepository.ChangePasswordAsync(adminInfo, newPassword);
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
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
