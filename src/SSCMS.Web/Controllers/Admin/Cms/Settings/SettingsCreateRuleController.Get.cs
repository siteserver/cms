using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleController
    {
        [HttpGet, Route(RouteGet)]
        public async Task<ActionResult<ChannelResult>> Get(int siteId, int channelId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                MenuUtils.SitePermissions.SettingsCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(channelId);

            var linkTypes = _pathManager.GetLinkTypeSelects();
            var filePath = string.IsNullOrEmpty(channel.FilePath) ? await _pathManager.GetInputChannelUrlAsync(site, channel, false) : channel.FilePath;
            var channelFilePathRule = string.IsNullOrEmpty(channel.ChannelFilePathRule) ? await _pathManager.GetChannelFilePathRuleAsync(site, channelId) : channel.ChannelFilePathRule;
            var contentFilePathRule = string.IsNullOrEmpty(channel.ContentFilePathRule) ? await _pathManager.GetContentFilePathRuleAsync(site, channelId) : channel.ContentFilePathRule;

            return new ChannelResult
            {
                Channel = channel,
                LinkTypes = linkTypes,
                FilePath = filePath,
                ChannelFilePathRule = channelFilePathRule,
                ContentFilePathRule = contentFilePathRule
            };
        }
    }
}