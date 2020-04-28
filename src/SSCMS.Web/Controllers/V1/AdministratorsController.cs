using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [Authorize(Roles = AuthTypes.Roles.Api)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class AdministratorsController : ControllerBase
    {
        private const string Route = "administrators";
        private const string RouteActionsLogin = "administrators/actions/login";
        private const string RouteActionsResetPassword = "administrators/actions/resetPassword";
        private const string RouteAdministrator = "administrators/{id:int}";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public AdministratorsController(IAuthManager authManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IDbCacheRepository dbCacheRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _dbCacheRepository = dbCacheRepository;
        }

        /// <summary>
        /// 新增管理员API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<Administrator>> Create([FromBody] Administrator request)
        {
            var isApiAuthorized = _authManager.IsApi && await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var (isValid, errorMessage) = await _administratorRepository.InsertAsync(request, request.Password);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return request;
        }

        /// <summary>
        /// 修改管理员API
        /// </summary>
        [HttpPut, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Update(int id, [FromBody] Administrator administrator)
        {
            var isApiAuthorized = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators);
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
            var isApiAuthorized = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await _administratorRepository.DeleteAsync(id);

            return administrator;
        }

        [HttpGet, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Get(int id)
        {
            var isApiAuthorized = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await _administratorRepository.GetByUserIdAsync(id);

            return administrator;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            var isApiAuthorized = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators);
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
            var (administrator, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);

            if (administrator == null)
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
            var accessToken = _authManager.AuthenticateAdministrator(administrator, request.IsAutoLogin);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(administrator.Id);
            await _dbCacheRepository.RemoveAndInsertAsync(cacheKey, sessionId);

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
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange
            };
        }

        [HttpPost, Route(RouteActionsResetPassword)]
        public async Task<ActionResult<Administrator>> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var isApiAuthorized = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators);
            if (!isApiAuthorized) return Unauthorized();

            var (administrator, _, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);
            if (administrator == null)
            {
                return this.Error(errorMessage);
            }

            bool isValid;
            (isValid, errorMessage) = await _administratorRepository.ChangePasswordAsync(administrator, request.NewPassword);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }
    }
}
