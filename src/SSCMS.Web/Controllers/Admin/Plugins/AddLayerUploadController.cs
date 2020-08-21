using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Plugins;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AddLayerUploadController : ControllerBase
    {
        private const string RouteActionsUpload = "plugins/addLayerUpload/actions/upload";
        private const string RouteActionsOverride = "plugins/addLayerUpload/actions/override";
        private const string RouteActionsRestart = "plugins/addLayerUpload/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;

        public AddLayerUploadController(IHostApplicationLifetime hostApplicationLifetime, IAuthManager authManager, IPathManager pathManager, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
        }

        public class UploadResult
        {
            public IPlugin OldPlugin { set; get; }
            public IPlugin NewPlugin { set; get; }
            public string FileName { set; get; }
        }

        public class OverrideRequest
        {
            public string PluginId { set; get; }
            public string FileName { set; get; }
        }
    }
}
