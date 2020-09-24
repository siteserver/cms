using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsErrorController : ControllerBase
    {
        private const string Route = "settings/logsError";

        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly IErrorLogRepository _errorLogRepository;

        public LogsErrorController(IAuthManager authManager, IPluginManager pluginManager, IErrorLogRepository errorLogRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _errorLogRepository = errorLogRepository;
        }

        public class SearchRequest : PageRequest
        {
            public string Category { get; set; }
            public string PluginId { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SearchResult : PageResult<ErrorLog>
        {
            public List<Select<string>> Categories { get; set; }
            public List<Select<string>> PluginIds { get; set; }
        }
    }
}
