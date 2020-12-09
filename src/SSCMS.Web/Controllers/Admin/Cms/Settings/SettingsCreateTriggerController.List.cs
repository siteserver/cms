using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateTriggerController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsCreateTrigger))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);

                var changeNames = new List<string>();
                var channelIdList = ListUtils.GetIntList(entity.CreateChannelIdsIfContentChanged);
                foreach (var channelId in channelIdList)
                {
                    if (await _channelRepository.IsExistsAsync(channelId))
                    {
                        changeNames.Add(await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, channelId));
                    }
                }

                return new
                {
                    entity.IndexName,
                    Count = count,
                    ChangeNames = changeNames,
                    entity.IsCreateChannelIfContentChanged,
                    CreateChannelIdsIfContentChanged = channelIdList,
                };
            });

            return new GetResult
            {
                Channel = cascade
            };
        }
    }
}