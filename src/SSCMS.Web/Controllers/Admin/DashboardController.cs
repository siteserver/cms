using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class DashboardController : ControllerBase
    {
        private const string Route = "dashboard";
        private const string RouteUnCheckedList = "dashboard/actions/unCheckedList";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public DashboardController(ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        public class GetResult
        {
            public string Version { get; set; }
            public string LastActivityDate { get; set; }
            public string UpdateDate { get; set; }
            public string AdminWelcomeHtml { get; set; }
            public string FrameworkDescription { get; set; }
            public string OSArchitecture { get; set; }
            public string OSDescription { get; set; }
            public bool Containerized { get; set; }
            public int CPUCores { get; set; }
            public string UserName { get; set; }
            public string Level { get; set; }
        }

        public class UnChecked
        {
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public int Count { get; set; }
        }

        public class GetUnCheckedListResult
        {
            public List<UnChecked> UnCheckedList { get; set; }
            public int TotalCount { get; set; }
        }
    }
}