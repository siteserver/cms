using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsUserController : ControllerBase
    {
        private const string Route = "settings/logsUser";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public LogsUserController(IAuthManager authManager, IUserRepository userRepository, ILogRepository logRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public class SearchRequest : PageRequest
        {
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }
    }
}
