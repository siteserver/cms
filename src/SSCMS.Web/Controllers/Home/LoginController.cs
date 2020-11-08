using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class LoginController : ControllerBase
    {
        private const string Route = "login";
        private const string RouteCaptcha = "login/captcha";
        private const string RouteCheckCaptcha = "login/captcha/actions/check";
        private const string RouteSendSms = "login/actions/sendSms";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISmsManager _smsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStatRepository _statRepository;

        public LoginController(ISettingsManager settingsManager, IAuthManager authManager, ICacheManager cacheManager, ISmsManager smsManager, IConfigRepository configRepository, IUserRepository userRepository, ILogRepository logRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _cacheManager = cacheManager;
            _smsManager = smsManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _statRepository = statRepository;
        }

        public class GetResult
        {
            public string HomeTitle { get; set; }
            public bool IsSmsEnabled { get; set; }
        }

        public class CheckRequest
        {
            public string Token { get; set; }
            public string Value { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsSmsLogin { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
            public string Mobile { get; set; }
            public string Code { get; set; }
            public bool IsPersistent { get; set; }
        }

        public class SubmitResult
        {
            public bool RedirectToVerifyMobile { get; set; }
            public User User { get; set; }
            public string Token { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return CacheUtils.GetClassKey(typeof(LoginController), nameof(User), mobile);
        }
    }
}
