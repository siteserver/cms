using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsConfigController : ControllerBase
    {
        private const string Route = "settings/logsConfig";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public LogsConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public Config Config { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsTimeThreshold { get; set; }
            public int TimeThreshold { get; set; }
            public bool IsLogSite { get; set; }

            public bool IsLogAdmin { get; set; }
            public bool IsLogUser { get; set; }
            public bool IsLogError { get; set; }
        }
    }
}
