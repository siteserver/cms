using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class LoginController : ControllerBase
    {
        private const string Route = "login";
        private const string RouteCaptcha = "login/captcha";
        private const string RouteCheckCaptcha = "login/captcha/actions/check";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public LoginController(ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository, IUserRepository userRepository, ILogRepository logRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");

            return new GetResult
            {
                HomeTitle = config.HomeTitle
            };
        }

        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            var (user, userName, errorMessage) = await _userRepository.ValidateAsync(request.Account, request.Password, true);

            if (user == null)
            {
                user = await _userRepository.GetByUserNameAsync(userName);
                if (user != null)
                {
                    await _logRepository.AddUserLogAsync(user, "用户登录失败", "帐号或密码错误");
                }
                return this.Error(errorMessage);
            }

            user = await _userRepository.GetByUserNameAsync(userName);
            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user
                ); // 记录最后登录时间、失败次数清零
            await _authManager.AddUserLogAsync("用户登录", string.Empty);

            var token = _authManager.AuthenticateUser(user, request.IsPersistent);

            return new LoginResult
            {
                User = user,
                Token = token
            };
        }
    }
}
