using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerColumnsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Types.ChannelPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            channelInfo.ListColumns = request.AttributeNames;

            await _channelRepository.UpdateAsync(channelInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "设置内容显示项", $"显示项:{request.AttributeNames}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
