using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpGet, Route(RoutePlugins)]
        public ActionResult<PluginsResult> Plugins([FromQuery] AgentRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            var allPlugins = _pluginManager.Plugins;

            var plugins = new List<AgentPlugin>();
            foreach (var plugin in allPlugins)
            {
                plugins.Add(new AgentPlugin
                {
                    Publisher = plugin.Publisher,
                    Name = plugin.Name,
                    Version = plugin.Version
                });
            }

            return new PluginsResult
            {
                Plugins = plugins,
            };
        }
    }
}