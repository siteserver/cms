using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerTranslateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Translate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            summaries.Reverse();

            foreach (var summary in summaries)
            {
                await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, TranslateType.Cut, _createManager, _authManager.AdminId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, "转移内容", string.Empty);

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await _createManager.TriggerContentChangedEventAsync(request.TransSiteId, request.TransChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}