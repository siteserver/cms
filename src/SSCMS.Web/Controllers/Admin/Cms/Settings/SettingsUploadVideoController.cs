using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsUploadVideoController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadVideo";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadVideoController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public string VideoUploadDirectoryName { get; set; }
            public DateFormatType VideoUploadDateFormatString { get; set; }
            public bool IsVideoUploadChangeFileName { get; set; }
            public string VideoUploadExtensions { get; set; }
            public int VideoUploadTypeMaxSize { get; set; }
        }
    }
}
