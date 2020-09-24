using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsCreateController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCreate";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsCreateController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public bool IsCreateDoubleClick { get; set; }
            public bool IsCreateContentIfContentChanged { get; set; }
            public bool IsCreateChannelIfChannelChanged { get; set; }
            public bool IsCreateShowPageInfo { get; set; }
            public bool IsCreateIe8Compatible { get; set; }
            public bool IsCreateBrowserNoCache { get; set; }
            public bool IsCreateJsIgnoreError { get; set; }
            public bool IsCreateWithJQuery { get; set; }
            public int CreateStaticMaxPage { get; set; }
            public bool IsCreateUseDefaultFileName { get; set; }
            public string CreateDefaultFileName { get; set; }
            public bool IsCreateStaticContentByAddDate { get; set; }
            public DateTime CreateStaticContentAddDate { get; set; }
        }
    }
}
