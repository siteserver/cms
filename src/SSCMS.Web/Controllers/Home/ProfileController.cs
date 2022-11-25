using System.Collections.Generic;
using Datory;
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
        private readonly ICloudManager _cloudManager;
        private readonly ISmsManager _smsManager;
        private readonly ICacheManager _cacheManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public ProfileController(IAuthManager authManager, IPathManager pathManager, ICloudManager cloudManager, ISmsManager smsManager, ICacheManager cacheManager, IConfigRepository configRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _cloudManager = cloudManager;
            _smsManager = smsManager;
            _cacheManager = cacheManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        public class Settings
        {
            public bool IsCloudImages { get; set; }
        }

        public class GetResult
        {
            public bool IsSmsEnabled { get; set; }
            public bool IsUserVerifyMobile { get; set; }
            public Entity Entity { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public Settings Settings { get; set; }
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
