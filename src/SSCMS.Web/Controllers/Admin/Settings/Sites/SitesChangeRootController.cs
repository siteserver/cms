using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesChangeRootController : ControllerBase
    {
        private const string Route = "settings/sitesChangeRoot";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SitesChangeRootController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public Site Site { get; set; }
            public List<string> Directories { get; set; }
            public List<string> Files { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public IList<string> CheckedDirectories { get; set; }
            public IList<string> CheckedFiles { get; set; }
            public bool IsMoveFiles { get; set; }
        }
    }
}
