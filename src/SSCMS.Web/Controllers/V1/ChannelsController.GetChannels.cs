using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;

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
            if (site == null) return NotFound();

            var channelInfoList = await _channelRepository.GetChannelsAsync(siteId);

            var dictInfoList = new List<IDictionary<string, object>>();
            foreach (var channelInfo in channelInfoList)
            {
                dictInfoList.Add(channelInfo.ToDictionary());
            }

            return dictInfoList;
        }
    }
}
