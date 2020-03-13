using System;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route(Constants.ApiRoute)]
    public partial class LoginController : ControllerBase
    {
        public const string Route = "login";
        private const string RouteCaptcha = "login/actions/captcha";

        private readonly ICacheManager<bool> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public LoginController(ICacheManager<bool> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, IDbCacheRepository dbCacheRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var redirectUrl = await AdminRedirectCheckAsync();
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return new GetResult
                {
                    Success = false,
                    RedirectUrl = redirectUrl
                };
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Success = true,
                ProductVersion = _settingsManager.ProductVersion,
                AdminTitle = config.AdminTitle
            };
        }

        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            

            Administrator adminInfo;

            var (isValid, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);

            if (!isValid)
            {
                adminInfo = await _administratorRepository.GetByUserNameAsync(userName);
                if (adminInfo != null)
                {
                    await _administratorRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(adminInfo); // 记录最后登录时间、失败次数+1
                }

                return this.Error(errorMessage);
            }

            adminInfo = await _administratorRepository.GetByUserNameAsync(userName);
            await _administratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(adminInfo); // 记录最后登录时间、失败次数清零
            var accessToken = await _authManager.AdminLoginAsync(adminInfo.UserName, request.IsAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(adminInfo.Id);
            await _dbCacheRepository.RemoveAndInsertAsync(cacheKey, sessionId);

            var config = await _configRepository.GetAsync();

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
                Administrator = adminInfo,
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange
            };
        }
    }
}