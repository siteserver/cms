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
    public partial class LogsSiteController : ControllerBase
    {
        private const string Route = "settings/logsSite";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteLogRepository _siteLogRepository;

        public LogsSiteController(IAuthManager authManager, IPathManager pathManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, ISiteLogRepository siteLogRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _siteLogRepository = siteLogRepository;
        }

        public class SearchRequest : PageRequest
        {
            public List<int> SiteIds { get; set; }
            public string LogType { get; set; }
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SiteLogResult : SiteLog
        {
            public string AdminName { get; set; }
            public string WebUrl { get; set; }

            public string SiteName { get; set; }
        }

        public class SiteLogPageResult : PageResult<SiteLogResult>
        {
            public IEnumerable<Cascade<int>> SiteOptions { get; set; }
        }
    }
}
