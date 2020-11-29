using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
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

        public class GetRequest
        {
            public string PluginIds { get; set; }
        }

        public class GetResult
        {
            public string CmsVersion { get; set; }
            public Dictionary<string, string> PluginPathDict { get; set; }
        }

        public class DownloadRequest
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
            public string Path { get; set; }
        }

        public class RestartRequest
        {
            public bool IsDisablePlugins { get; set; }
        }

        public class UploadRequest
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
        }
    }
}
