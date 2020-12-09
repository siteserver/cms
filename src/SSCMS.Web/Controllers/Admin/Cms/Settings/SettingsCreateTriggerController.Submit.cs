using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateTriggerController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsCreateTrigger))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            channel.IsCreateChannelIfContentChanged = request.IsCreateChannelIfContentChanged;
            channel.CreateChannelIdsIfContentChanged = ListUtils.ToString(request.CreateChannelIdsIfContentChanged);

            await _channelRepository.UpdateAsync(channel);

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "设置栏目变动生成页面", $"栏目:{channel.ChannelName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}