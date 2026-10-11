using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.AdminConfig
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdminConfigController : ControllerBase
    {
        private const string Route = "settings/adminConfig";
        private const string RouteUpload = "settings/adminConfig/actions/upload";

        // 旧路由别名（原「云助手 - 后台版权设置」），保持兼容
        private const string LegacyRoute = "clouds/admin";
        private const string LegacyRouteUpload = "clouds/admin/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;

        public AdminConfigController(
            IAuthManager authManager,
            IPathManager pathManager,
            IConfigRepository configRepository
        )
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
            public string AdminLogoLinkUrl { get; set; }
            public string AdminWelcomeHtml { get; set; }
            public bool IsAdminUpdateDisabled { get; set; }
            public bool IsAdminTicketsDisabled { get; set; }
            public bool IsAdminWatermark { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsCloudAdmin { get; set; }
            public string AdminTitle { get; set; }
            public string AdminFaviconUrl { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminLogoLinkUrl { get; set; }
            public string AdminWelcomeHtml { get; set; }
            public bool IsAdminUpdateDisabled { get; set; }
            public bool IsAdminTicketsDisabled { get; set; }
            public bool IsAdminWatermark { get; set; }
        }

        public class UploadResult
        {
            public string Type { get; set; }
            public string Url { get; set; }
        }
    }
}
