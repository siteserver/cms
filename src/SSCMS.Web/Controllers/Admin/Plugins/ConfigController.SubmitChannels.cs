using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Plugins;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class ConfigController
    {
        [HttpPost, Route(RouteActionsSubmitChannels)]
        public async Task<ActionResult<BoolResult>> SubmitChannels([FromBody] SubmitChannelsRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = _pluginManager.GetPlugin(request.PluginId);
            var siteConfigs = (plugin.SiteConfigs ?? new List<SiteConfig>()).ToList();
            var siteConfig = siteConfigs.FirstOrDefault(x => x.SiteId == request.SiteId);
            if (siteConfig != null)
            {
                siteConfig.AllChannels = request.AllChannels;
                siteConfig.ChannelIds = request.ChannelIds;
            }
            else
            {
                siteConfig = new SiteConfig
                {
                    SiteId = request.SiteId,
                    AllChannels = request.AllChannels,
                    ChannelIds = request.ChannelIds
                };
                siteConfigs.Add(siteConfig);
            }

            var config = await _pluginManager.GetConfigAsync(request.PluginId);
            config[nameof(IPlugin.SiteConfigs)] = siteConfigs;
            await _pluginManager.SaveConfigAsync(request.PluginId, config);

            await _authManager.AddAdminLogAsync("修改插件配置", $"插件:{request.PluginId}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}