using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [OpenApiOperation("获取栏目列表 API", "获取栏目列表使用 GET 发起请求，请求地址为 /api/v1/channels/{siteId}")]
        [HttpGet, Route(RouteSite)]
        public async Task<ActionResult<List<IDictionary<string, object>>>> GetChannels([FromRoute] int siteId)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channels = await _channelRepository.GetChannelsAsync(siteId);

            var dictList = new List<IDictionary<string, object>>();
            foreach (var channel in channels)
            {
                var dict = channel.ToDictionary();

                var navigationUrl = await _pathManager.GetChannelUrlAsync(site, channel, false);
                dict[nameof(ColumnsManager.NavigationUrl)] = navigationUrl;

                var imageUrl = string.Empty;
                if (!string.IsNullOrEmpty(channel.ImageUrl))
                {
                    imageUrl = await _pathManager.ParseSiteUrlAsync(site, channel.ImageUrl, true);
                    dict[nameof(channel.ImageUrl)] = imageUrl;
                }

                dictList.Add(dict);
            }

            return dictList;
        }
    }
}
