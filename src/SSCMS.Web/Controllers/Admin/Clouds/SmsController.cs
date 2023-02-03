using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SmsController : ControllerBase
    {
        private const string Route = "clouds/sms";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly IConfigRepository _configRepository;

        public SmsController(IAuthManager authManager, ICloudManager cloudManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public bool IsCloudSms { get; set; }
            public bool IsCloudSmsAdmin { get; set; }
            public bool IsCloudSmsAdminAndDisableAccount { get; set; }
            public bool IsCloudSmsUser { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsCloudSms { get; set; }
            public bool IsCloudSmsAdmin { get; set; }
            public bool IsCloudSmsAdminAndDisableAccount { get; set; }
            public bool IsCloudSmsUser { get; set; }
        }
    }
}
