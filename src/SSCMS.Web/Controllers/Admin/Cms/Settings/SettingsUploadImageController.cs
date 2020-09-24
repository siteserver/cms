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
    public partial class SettingsUploadImageController : ControllerBase
    {
        private const string Route = "cms/settings/settingsUploadImage";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsUploadImageController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public string ImageUploadDirectoryName { get; set; }
            public DateFormatType ImageUploadDateFormatString { get; set; }
            public bool IsImageUploadChangeFileName { get; set; }
            public string ImageUploadExtensions { get; set; }
            public int ImageUploadTypeMaxSize { get; set; }
            public int PhotoSmallWidth { get; set; }
            public int PhotoMiddleWidth { get; set; }
        }
    }
}
