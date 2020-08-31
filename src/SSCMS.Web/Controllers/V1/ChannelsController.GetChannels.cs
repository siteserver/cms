using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [HttpGet, Route(RouteSite)]
        public async Task<ActionResult<List<IDictionary<string, object>>>> GetChannels(int siteId)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels) ||
                         _authManager.IsAdmin;
            if (!isAuth) return Unauthorized();

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
