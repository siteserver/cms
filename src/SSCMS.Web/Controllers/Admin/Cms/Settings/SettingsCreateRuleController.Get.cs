using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using System.Collections.Generic;

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

            var linkTypes = _pathManager.GetLinkTypeSelects(true);
            var filePath = string.IsNullOrEmpty(channel.FilePath) ? await _pathManager.GetInputChannelUrlAsync(site, channel, false) : channel.FilePath;
            var channelFilePathRule = string.IsNullOrEmpty(channel.ChannelFilePathRule) ? await _pathManager.GetChannelFilePathRuleAsync(site, channelId) : channel.ChannelFilePathRule;
            var contentFilePathRule = string.IsNullOrEmpty(channel.ContentFilePathRule) ? await _pathManager.GetContentFilePathRuleAsync(site, channelId) : channel.ContentFilePathRule;

            var linkTo = new LinkTo
            {
                ChannelIds = new List<int> {
                  siteId,
                },
                ContentId = 0,
                ContentTitle = string.Empty
            };
            if (channel.LinkType == Enums.LinkType.LinkToChannel)
            {
                linkTo.ChannelIds = ListUtils.GetIntList(channel.LinkUrl);
            }
            else if (channel.LinkType == Enums.LinkType.LinkToContent)
            {
                if (!string.IsNullOrEmpty(channel.LinkUrl) && channel.LinkUrl.IndexOf('_') != -1)
                {
                    var arr = channel.LinkUrl.Split('_');
                    if (arr.Length == 2)
                    {
                        var channelIds = ListUtils.GetIntList(arr[0]);
                        var linkContentId = TranslateUtils.ToInt(arr[1]);
                        var linkChannelId = channelIds.Count > 0 ? channelIds[channelIds.Count - 1] : 0;
                        var linkToContent = await _contentRepository.GetAsync(site.Id, linkChannelId, linkContentId);
                        if (linkToContent != null)
                        {
                            linkTo.ChannelIds = channelIds;
                            linkTo.ContentId = linkContentId;
                            linkTo.ContentTitle = linkToContent.Title;
                        }
                    }
                }
            }

            return new ChannelResult
            {
                Channel = channel,
                LinkTypes = linkTypes,
                FilePath = filePath,
                ChannelFilePathRule = channelFilePathRule,
                ContentFilePathRule = contentFilePathRule,
                LinkTo = linkTo,
            };
        }
    }
}