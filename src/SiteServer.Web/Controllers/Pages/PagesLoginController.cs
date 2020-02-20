using System;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.Web.Core;

namespace SiteServer.Web.Controllers.Pages
{
    [Route("pages/login")]
    public partial class PagesLoginController : ControllerBase
    {
        private const string Route = "";

        private readonly ICacheManager<bool> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public PagesLoginController(ICacheManager<bool> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<GetResult> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            var redirectUrl = await auth.AdminRedirectCheckAsync(checkInstall: true, checkDatabaseVersion: true);
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
            var auth = await _authManager.GetAdminAsync();

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
            var accessToken = await auth.AdminLoginAsync(adminInfo.UserName, request.IsAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(adminInfo.Id);
            CacheUtils.Insert(cacheKey, sessionId);

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