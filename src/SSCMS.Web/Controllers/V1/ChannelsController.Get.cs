using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [OpenApiOperation("获取栏目 API", "获取栏目，使用GET发起请求，请求地址为/api/v1/channels/{siteId}/{channelId}")]
        [HttpGet, Route(RouteChannel)]
        public async Task<ActionResult<Dictionary<string, object>>> Get([FromRoute] int siteId, [FromRoute] int channelId)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null)
            {
                return this.Error(Constants.ErrorNotFound);
            }

            if (!await _authManager.HasSitePermissionsAsync(siteId, MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null)
            {
                return this.Error(Constants.ErrorNotFound);
            }

            var retVal = channel.ToDictionary();

            var navigationUrl = await _pathManager.GetChannelUrlAsync(site, channel, false);
            retVal[nameof(ColumnsManager.NavigationUrl)] = navigationUrl;

            if (!string.IsNullOrEmpty(channel.ImageUrl))
            {
                var imageUrl = await _pathManager.ParseSiteUrlAsync(site, channel.ImageUrl, true);
                retVal[nameof(channel.ImageUrl)] = imageUrl;
            }

            // channel.Children = await _channelRepository.GetChildrenAsync(siteId, channelId);

            var summaries = await _channelRepository.GetSummariesAsync(site.Id);
            retVal[nameof(channel.Children)] = await GetChildrenAsync(site, summaries, channelId);

            // retVal[nameof(channel.Children)] = await _channelRepository.GetCascadeChildrenAsync(site, channel.Id, async summary =>
            // {
            //     var channel = await _channelRepository.GetAsync(summary.Id);

            //     var dict = channel.ToDictionary();

            //     var navigationUrl = await _pathManager.GetChannelUrlAsync(site, channel, false);
            //     dict[nameof(ColumnsManager.NavigationUrl)] = navigationUrl;

            //     var imageUrl = string.Empty;
            //     if (!string.IsNullOrEmpty(channel.ImageUrl))
            //     {
            //         imageUrl = await _pathManager.ParseSiteUrlAsync(site, channel.ImageUrl, true);
            //         dict[nameof(channel.ImageUrl)] = imageUrl;
            //     }

            //     return dict;
            // });

            return (Dictionary<string, object>)retVal;
        }

        private async Task<List<IDictionary<string, object>>> GetChildrenAsync(Site site, List<ChannelSummary> summaries, int parentId)
        {
            var list = new List<IDictionary<string, object>>();

            foreach (var summary in summaries)
            {
                if (summary == null) continue;
                if (summary.ParentId == parentId)
                {
                    var channel = await _channelRepository.GetAsync(summary.Id);
                    var dict = channel.ToDictionary();
                    var navigationUrl = await _pathManager.GetChannelUrlAsync(site, channel, false);
                    dict[nameof(ColumnsManager.NavigationUrl)] = navigationUrl;
                    if (!string.IsNullOrEmpty(channel.ImageUrl))
                    {
                        var imageUrl = await _pathManager.ParseSiteUrlAsync(site, channel.ImageUrl, true);
                        dict[nameof(channel.ImageUrl)] = imageUrl;
                    }

                    dict[nameof(channel.Children)] = await GetChildrenAsync(site, summaries, channel.Id);
                    list.Add(dict);
                }
            }

            return list;
        }
    }
}
