using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ProfileController : ControllerBase
    {
        private const string Route = "profile";
        private const string RouteUpload = "profile/actions/upload";
        private const string RouteSendSms = "profile/actions/sendSms";
        private const string RouteVerifyMobile = "profile/actions/verifyMobile";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISmsManager _smsManager;
        private readonly ICacheManager _cacheManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ProfileController(IAuthManager authManager, IPathManager pathManager, ISmsManager smsManager, ICacheManager cacheManager, IConfigRepository configRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _smsManager = smsManager;
            _cacheManager = cacheManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public bool IsSmsEnabled { get; set; }
            public bool IsUserVerifyMobile { get; set; }
            public User User { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        public class VerifyMobileRequest
        {
            public string Mobile { get; set; }
            public string Code { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return CacheUtils.GetClassKey(typeof(ProfileController), nameof(User), mobile);
        }
    }
}
