using System.Collections.Generic;
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
    public partial class ManageController : ControllerBase
    {
        private const string Route = "plugins/manage";
        private const string RouteActionsDisable = "plugins/manage/actions/disable";
        private const string RouteActionsDelete = "plugins/manage/actions/delete";
        private const string RouteActionsRestart = "plugins/manage/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public ManageController(IHostApplicationLifetime hostApplicationLifetime, ISettingsManager settingsManager, IAuthManager authManager, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        public class GetResult
        {
            public string CmsVersion { get; set; }
            public IEnumerable<IPlugin> AllPlugins { get; set; }
            public bool Containerized { get; set; }
        }

        public class DisableRequest
        {
            public string PluginId { get; set; }
            public bool Disabled { get; set; }
        }

        public class DeleteRequest
        {
            public string PluginId { get; set; }
        }
    }
}
