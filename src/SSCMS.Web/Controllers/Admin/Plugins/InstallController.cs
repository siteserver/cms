using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class InstallController : ControllerBase
    {
        private const string Route = "plugins/install";
        private const string RouteActionsDownload = "plugins/install/actions/download";
        private const string RouteActionsUpdate = "plugins/install/actions/update";
        private const string RouteActionsRestart = "plugins/install/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public InstallController(IHostApplicationLifetime hostApplicationLifetime, ISettingsManager settingsManager,
            IAuthManager authManager, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
        }

        public class DownloadRequest
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
        }

        public class UploadRequest
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
        }
    }
}
