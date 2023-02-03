using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdminController : ControllerBase
    {
        private const string Route = "clouds/admin";
        private const string RouteUpload = "clouds/admin/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;

        public AdminController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
        }

        public class GetResult
        {
            public bool IsCloudAdmin { get; set; }
            public string AdminTitle { get; set; }
            public string AdminFaviconUrl { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminWelcomeHtml { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsCloudAdmin { get; set; }
            public string AdminTitle { get; set; }
            public string AdminFaviconUrl { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminLogoLinkUrl { get; set; }
            public string AdminWelcomeHtml { get; set; }
        }

        public class UploadResult
        {
            public string Type { get; set; }
            public string Url { get; set; }
        }
    }
}
