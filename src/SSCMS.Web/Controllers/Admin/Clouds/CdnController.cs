using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CdnController : ControllerBase
    {
        private const string Route = "clouds/cdn";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly IConfigRepository _configRepository;
        private readonly IScheduledTaskRepository _scheduledTaskRepository;

        public CdnController(IAuthManager authManager, ICloudManager cloudManager, IConfigRepository configRepository, IScheduledTaskRepository scheduledTaskRepository)
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
            _configRepository = configRepository;
            _scheduledTaskRepository = scheduledTaskRepository;
        }

        public class GetResult
        {
            public CloudType CloudType { get; set; }
            public bool IsCloudCdn { get; set; }
            public bool IsCloudCdnImages { get; set; }
            public bool IsCloudCdnFiles { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsCloudCdn { get; set; }
            public bool IsCloudCdnImages { get; set; }
            public bool IsCloudCdnFiles { get; set; }
        }
    }
}
