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
    public partial class CensorController : ControllerBase
    {
        private const string Route = "clouds/censor";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly IConfigRepository _configRepository;

        public CensorController(IAuthManager authManager, ICloudManager cloudManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public bool IsCloudCensorText { get; set; }
            public bool IsCloudCensorTextAuto { get; set; }
            public bool IsCloudCensorTextIgnore { get; set; }
            public bool IsCloudCensorTextWhiteList { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsCloudCensorText { get; set; }
            public bool IsCloudCensorTextAuto { get; set; }
            public bool IsCloudCensorTextIgnore { get; set; }
            public bool IsCloudCensorTextWhiteList { get; set; }
        }
    }
}
