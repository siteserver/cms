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
    public partial class SettingsUploadAudioController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadAudio";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadAudioController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public string AudioUploadDirectoryName { get; set; }
            public DateFormatType AudioUploadDateFormatString { get; set; }
            public bool IsAudioUploadChangeFileName { get; set; }
            public string AudioUploadExtensions { get; set; }
            public int AudioUploadTypeMaxSize { get; set; }
        }
    }
}
