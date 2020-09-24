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
    public partial class SettingsContentController : ControllerBase
    {
        private const string Route = "cms/settings/settingsContent";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsContentController(IAuthManager authManager, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public bool IsSaveImageInTextEditor { get; set; }
            public int PageSize { get; set; }
            public bool IsAutoPageInTextEditor { get; set; }
            public int AutoPageWordNum { get; set; }
            public bool IsContentTitleBreakLine { get; set; }
            public bool IsContentSubTitleBreakLine { get; set; }
            public bool IsAutoCheckKeywords { get; set; }
            public int CheckContentLevel { get; set; }
            public int CheckContentDefaultLevel { get; set; }
        }
    }
}
