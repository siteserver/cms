using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class VerifyMobileController : ControllerBase
    {
        private const string Route = "verifyMobile";
        private const string RouteSendSms = "verifyMobile/actions/sendSms";

        private readonly IAuthManager _authManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISmsManager _smsManager;
        private readonly IUserRepository _userRepository;

        public VerifyMobileController(IAuthManager authManager, ICacheManager cacheManager, ISmsManager smsManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _cacheManager = cacheManager;
            _smsManager = smsManager;
            _userRepository = userRepository;
        }

        public class GetResult
        {
            public string Mobile { get; set; }
        }

        public class SubmitRequest
        {
            public string Mobile { get; set; }
            public string Code { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return CacheUtils.GetClassKey(typeof(VerifyMobileController), nameof(User), mobile);
        }
    }
}
