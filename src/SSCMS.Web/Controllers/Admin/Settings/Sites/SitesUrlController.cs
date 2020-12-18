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
    public partial class SitesUrlController : ControllerBase
    {
        private const string Route = "settings/sitesUrl";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SitesUrlController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public List<Site> Sites { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public bool IsSeparatedWeb { get; set; }
            public string SeparatedWebUrl { get; set; }
            public bool IsSeparatedAssets { get; set; }
            public string AssetsDir { get; set; }
            public string SeparatedAssetsUrl { get; set; }
            public bool IsSeparatedApi { get; set; }
            public string SeparatedApiUrl { get; set; }
        }

        public class SubmitResult
        {
            public List<Site> Sites { get; set; }
        }
    }
}
