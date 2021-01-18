using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<ListResult>> List([FromBody] ListRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId > 0 ? request.ChannelId : request.SiteId);

            var isAdd = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Add);
            var pageContents = new List<Content>();

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);

            var offset = site.PageSize * (request.Page - 1);

            int? channelId = null;
            if (channel.Id != request.SiteId)
            {
                channelId = channel.Id;
            }
            var (total, pageSummaries) = await _contentRepository.UserWriteSearchAsync(_authManager.UserId, site, request.Page, channelId, request.IsCheckedLevels, request.CheckedLevels, request.GroupNames, request.TagNames);

            if (total > 0)
            {
                var sequence = offset + 1;
                foreach (var summary in pageSummaries)
                {
                    var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                    if (content == null) continue;

                    var pageContent =
                        await columnsManager.CalculateContentListAsync(sequence++, site, request.SiteId, content, columns);

                    pageContents.Add(pageContent);
                }
            }

            return new ListResult
            {
                IsAdd = isAdd,
                PageContents = pageContents,
                Total = total,
                PageSize = site.PageSize
            };
        }
    }
}
