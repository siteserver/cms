using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Extensions;

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
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsController(IAuthManager authManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<Administrator>> Create([FromBody] Administrator request)
        {
            var isApiAuthorized = await _authManager.IsApiAuthenticatedAsync() && await _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var (isValid, errorMessage) = await _administratorRepository.InsertAsync(request, request.Password);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return request;
        }

        [HttpPut, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Update(int id, [FromBody] Administrator administrator)
        {
            var isApiAuthorized = await _authManager.IsApiAuthenticatedAsync() && await _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (administrator == null) return this.Error("Could not read administrator from body");

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            administrator.Id = id;

            var (isValid, errorMessage) = await _administratorRepository.UpdateAsync(administrator);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }

        [HttpDelete, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Delete(int id)
        {
            var isApiAuthorized = await _authManager.IsApiAuthenticatedAsync() && await _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await _administratorRepository.DeleteAsync(id);

            return administrator;
        }

        [HttpGet, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Get(int id)
        {
            var isApiAuthorized = await _authManager.IsApiAuthenticatedAsync() && await _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await _administratorRepository.GetByUserIdAsync(id);

            return administrator;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            var isApiAuthorized = await _authManager.IsApiAuthenticatedAsync() && await _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var top = request.Top;
            if (top <= 0) top = 20;
            var skip = request.Skip;

            var administrators = await _administratorRepository.GetAdministratorsAsync(skip, top);
            var count = await _administratorRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Administrators = administrators
            };
        }

        [HttpPost, Route(RouteActionsLogin)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            Administrator administrator;

            var (isValid, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);

            if (!isValid)
            {
                administrator = await _administratorRepository.GetByUserNameAsync(userName);
                if (administrator != null)
                {
                    await _administratorRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(administrator); // 记录最后登录时间、失败次数+1
                }

                return this.Error(errorMessage);
            }

            administrator = await _administratorRepository.GetByUserNameAsync(userName);
            await _administratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(administrator); // 记录最后登录时间、失败次数清零
            var accessToken = await _authManager.AdminLoginAsync(administrator.UserName, request.IsAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(administrator.Id);
            CacheUtils.Insert(cacheKey, sessionId);

            var config = await _configRepository.GetAsync();

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
            var administrator = await _authManager.GetAdminAsync();
            _authManager.AdminLogout();

            return administrator;
        }

        [HttpPost, Route(RouteActionsResetPassword)]
        public async Task<ActionResult<Administrator>> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var isApiAuthorized = await _authManager.IsApiAuthenticatedAsync() && await _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var (isValid, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            var administrator = await _administratorRepository.GetByUserNameAsync(userName);

            (isValid, errorMessage) = await _administratorRepository.ChangePasswordAsync(administrator, request.NewPassword);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }
    }
}
