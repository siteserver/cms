using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();

            if (siteIds.Count == 0)
            {
                return new GetResult
                {
                    Unauthorized = true
                };
            }

            var sites = await _siteRepository.GetCascadeChildrenAsync(0, summary =>
            {
                return new
                {
                    Disabled = !siteIds.Contains(summary.Id),
                };
            });

            var site = await _siteRepository.GetAsync(siteIds.Contains(request.SiteId) ? request.SiteId : siteIds[0]);

            var channel = await _channelRepository.GetAsync(site.Id);
            var enabledChannelIds = await _authManager.GetContentPermissionsChannelIdsAsync(site.Id);
            var visibleChannelIds = await _authManager.GetVisibleChannelIdsAsync(enabledChannelIds);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var visible = visibleChannelIds.Contains(summary.Id);
                var disabled = !enabledChannelIds.Contains(summary.Id);
                var current = await _contentRepository.GetSummariesAsync(site, summary);

                if (!visible) return null;

                return new
                {
                    current.Count,
                    Disabled = disabled
                };
            });

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);
            var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);

            var titleColumn =
                columns.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.AttributeName, nameof(Models.Content.Title)));
            columns.Remove(titleColumn);

            return new GetResult
            {
                Unauthorized = false,
                Sites = sites,
                SiteId = site.Id,
                SiteName = site.SiteName,
                SiteUrl = siteUrl,
                Root = root,
                GroupNames = groupNames,
                TagNames = tagNames,
                CheckedLevels = checkedLevels,
                TitleColumn = titleColumn,
                Columns = columns
            };
        }
    }
}
