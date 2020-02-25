using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.V1
{
    [Route("v1/administrators")]
    public partial class AdministratorsController : ControllerBase
    {
        private const string Route = "";
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsLogout = "actions/logout";
        private const string RouteActionsResetPassword = "actions/resetPassword";
        private const string RouteAdministrator = "{id:int}";

        private readonly IAuthManager _authManager;

        public AdministratorsController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<Administrator>> Create([FromBody] Administrator request)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var (isValid, errorMessage) = await DataProvider.AdministratorRepository.InsertAsync(request, request.Password);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return request;
        }

        [HttpPut, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Update(int id, [FromBody] Administrator administrator)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (administrator == null) return this.Error("Could not read administrator from body");

            if (!await DataProvider.AdministratorRepository.IsExistsAsync(id)) return NotFound();

            administrator.Id = id;

            var (isValid, errorMessage) = await DataProvider.AdministratorRepository.UpdateAsync(administrator);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }

        [HttpDelete, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Delete(int id)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (!await DataProvider.AdministratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await DataProvider.AdministratorRepository.DeleteAsync(id);

            return administrator;
        }

        [HttpGet, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Get(int id)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (!await DataProvider.AdministratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await DataProvider.AdministratorRepository.GetByUserIdAsync(id);

            return administrator;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var top = request.Top;
            if (top <= 0) top = 20;
            var skip = request.Skip;

            var administrators = await DataProvider.AdministratorRepository.GetAdministratorsAsync(skip, top);
            var count = await DataProvider.AdministratorRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Administrators = administrators
            };
        }

        [HttpPost, Route(RouteActionsLogin)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            Administrator administrator;

            var (isValid, userName, errorMessage) = await DataProvider.AdministratorRepository.ValidateAsync(request.Account, request.Password, true);

            if (!isValid)
            {
                administrator = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
                if (administrator != null)
                {
                    await DataProvider.AdministratorRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(administrator); // 记录最后登录时间、失败次数+1
                }

                return this.Error(errorMessage);
            }

            administrator = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
            await DataProvider.AdministratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(administrator); // 记录最后登录时间、失败次数清零
            var accessToken = await auth.AdminLoginAsync(administrator.UserName, request.IsAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(administrator.Id);
            CacheUtils.Insert(cacheKey, sessionId);

            var config = await DataProvider.ConfigRepository.GetAsync();

            var isEnforcePasswordChange = false;
            if (config.IsAdminEnforcePasswordChange)
            {
                if (administrator.LastChangePasswordDate == null)
                {
                    isEnforcePasswordChange = true;
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - administrator.LastChangePasswordDate.Value.Ticks);
                    if (ts.TotalDays > config.AdminEnforcePasswordChangeDays)
                    {
                        isEnforcePasswordChange = true;
                    }
                }
            }

            return new LoginResult
            {
                Administrator = administrator,
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange
            };
        }

        [HttpPost, Route(RouteActionsLogout)]
        public async Task<Administrator> Logout()
        {
            var auth = await _authManager.GetApiAsync();

            var administrator = auth.IsAdminLoggin ? auth.Administrator : null;
            auth.AdminLogout();

            return administrator;
        }

        [HttpPost, Route(RouteActionsResetPassword)]
        public async Task<ActionResult<Administrator>> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            var isApiAuthorized = auth.IsApiAuthenticated && await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var (isValid, userName, errorMessage) = await DataProvider.AdministratorRepository.ValidateAsync(request.Account, request.Password, true);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            var administrator = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);

            (isValid, errorMessage) = await DataProvider.AdministratorRepository.ChangePasswordAsync(administrator, request.NewPassword);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }
    }
}
