using System.Collections.Generic;
using Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsSiteController : ControllerBase
    {
        private const string Route = "cms/settings/settingsSite";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public SettingsSiteController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public string SiteUrl { get; set; }
            public Site Site { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class SubmitRequest : Entity
        {
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string ImageUrl { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
        }
    }
}
