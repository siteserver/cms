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
            if (request == null)
            {
                return this.Error("系统参数不足");
            }
            if (!TryValidateAgentSecurityKey(request.SecurityKey, out var securityKeyErrorMessage))
            {
                return this.Error(securityKeyErrorMessage);
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
