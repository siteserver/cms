using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LostPasswordController : ControllerBase
    {
        public const string Route = "lostPassword";
        private const string RouteSendSms = "lostPassword/actions/sendSms";

        private readonly IAuthManager _authManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISmsManager _smsManager;
        private readonly IAdministratorRepository _administratorRepository;

        public LostPasswordController(IAuthManager authManager, ICacheManager cacheManager, ISmsManager smsManager, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _cacheManager = cacheManager;
            _smsManager = smsManager;
            _administratorRepository = administratorRepository;
        }

        public class SubmitRequest
        {
            public string Mobile { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }
        }

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        private string GetSmsCodeCacheKey(string mobile)
        {
            return CacheUtils.GetClassKey(typeof(LostPasswordController), nameof(Administrator), mobile);
        }
    }
}