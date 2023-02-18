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
    [AutoValidateAntiforgeryToken]
    public partial class SettingsUploadAudioController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadAudio";
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadAudioController(ISettingsManager settingsManager, IAuthManager authManager, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public string CSRFToken { get; set; }
            public bool IsSafeMode { get; set; }
            public string AudioUploadDirectoryName { get; set; }
            public DateFormatType AudioUploadDateFormatString { get; set; }
            public bool IsAudioUploadChangeFileName { get; set; }
            public string AudioUploadExtensions { get; set; }
            public long AudioUploadTypeMaxSize { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string AudioUploadDirectoryName { get; set; }
            public DateFormatType AudioUploadDateFormatString { get; set; }
            public bool IsAudioUploadChangeFileName { get; set; }
            public string AudioUploadExtensions { get; set; }
            public long AudioUploadTypeMaxSize { get; set; }
        }
    }
}
