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
            var agentCheck = CheckAgent(request.SecurityKey);
            if (agentCheck != null) return agentCheck;

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