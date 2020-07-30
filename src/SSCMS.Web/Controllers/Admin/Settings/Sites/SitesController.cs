using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesController : ControllerBase
    {
        private const string Route = "settings/sites";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _oldPluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public SitesController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IOldPluginManager oldPluginManager, ISiteRepository siteRepository,
            IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _oldPluginManager = oldPluginManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        public class GetResult
        {
            public IEnumerable<SiteType> SiteTypes { get; set; }
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
            public List<string> TableNames { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public bool DeleteFiles { get; set; }
        }

        public class EditRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public string SiteName { get; set; }
            public string SiteType { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
            public TableRule TableRule { get; set; }
            public string TableChoose { get; set; }
            public string TableHandWrite { get; set; }
        }

        public class SitesResult
        {
            public List<Site> Sites { get; set; }
        }
    }
}
