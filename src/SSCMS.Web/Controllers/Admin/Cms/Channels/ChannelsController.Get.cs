using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpGet, Route(RouteGet)]
        public async Task<ActionResult<ChannelResult>> Get(int siteId, int channelId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                    Types.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);

            var styles = await GetInputStylesAsync(channel);
            var entity = new Entity(channel.ToDictionary());
            foreach (var style in styles)
            {
                if (style.InputType == InputType.Image ||
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    var count = channel.Get(ColumnsManager.GetCountName(style.AttributeName), 0);
                    entity.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, n);
                        entity.Set(extendName, channel.Get(extendName));
                    }
                }
                else if (style.InputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = ListUtils.GetStringList(channel.Get(style.AttributeName,
                        string.Empty));
                    entity.Set(style.AttributeName, list);
                }
                else
                {
                    entity.Set(style.AttributeName, channel.Get(style.AttributeName));
                }
            }

            var filePath = string.IsNullOrEmpty(channel.FilePath) ? await _pathManager.GetInputChannelUrlAsync(site, channel, false) : channel.FilePath;
            var channelFilePathRule = string.IsNullOrEmpty(channel.ChannelFilePathRule) ? await _pathManager.GetChannelFilePathRuleAsync(site, channelId) : channel.ChannelFilePathRule;
            var contentFilePathRule = string.IsNullOrEmpty(channel.ContentFilePathRule) ? await _pathManager.GetContentFilePathRuleAsync(site, channelId) : channel.ContentFilePathRule;

            return new ChannelResult
            {
                Entity = entity,
                Styles = styles,
                FilePath = filePath,
                ChannelFilePathRule = channelFilePathRule,
                ContentFilePathRule = contentFilePathRule
            };
        }
    }
}