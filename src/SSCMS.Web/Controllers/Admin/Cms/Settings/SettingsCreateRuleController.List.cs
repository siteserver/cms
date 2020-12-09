using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsCreateRule))
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
                var filePath = await _pathManager.GetInputChannelUrlAsync(site, entity, false);
                var contentFilePathRule = string.IsNullOrEmpty(entity.ContentFilePathRule)
                    ? await _pathManager.GetContentFilePathRuleAsync(site, summary.Id)
                    : entity.ContentFilePathRule;
                return new
                {
                    entity.IndexName,
                    Count = count,
                    FilePath = filePath,
                    ContentFilePathRule = contentFilePathRule
                };
            });

            return new GetResult
            {
                Channel = cascade
            };
        }
    }
}