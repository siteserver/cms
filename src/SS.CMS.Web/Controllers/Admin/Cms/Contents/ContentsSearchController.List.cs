using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsSearchController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<ListResult>> List([FromBody] ListRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ContentsSearch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);

            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = _pluginManager.GetContentColumns(pluginIds);

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);

            var offset = site.PageSize * (request.Page - 1);
            int total;
            List<ContentSummary> pageSummaries;
            if (request.IsAdvanced)
            {
                var isAdmin = false;
                var adminId = await _authManager.GetAdminIdAsync();
                var isUser = false;
                if (request.SearchType == SearchType.Admin)
                {
                    isAdmin = true;
                }
                else if (request.SearchType == SearchType.User)
                {
                    isUser = true;
                }

                (total, pageSummaries) = await _contentRepository.AdvancedSearch(site, request.Page, request.ChannelIds, request.IsAllContents, request.StartDate, request.EndDate, request.Items, request.IsCheckedLevels, request.CheckedLevels, request.IsTop, request.IsRecommend, request.IsHot, request.IsColor, request.GroupNames, request.TagNames, isAdmin, adminId, isUser);
            }
            else
            {
                var channelId = request.ChannelIds.FirstOrDefault();
                var first = await _channelRepository.GetAsync(channelId);
                var summaries = await _contentRepository.GetSummariesAsync(site, first, request.IsAllContents);

                total = summaries.Count;
                pageSummaries = summaries.Skip(offset).Take(site.PageSize).ToList();
            }

            var pageContents = new List<Content>();
            if (total > 0)
            {
                var sequence = offset + 1;
                foreach (var summary in pageSummaries)
                {
                    var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                    if (content == null) continue;

                    var pageContent =
                        await columnsManager.CalculateContentListAsync(sequence++, site, request.SiteId, content, columns, pluginColumns);

                    var menus = await _pluginManager.GetContentMenusAsync(pluginIds, pageContent);
                    pageContent.Set("PluginMenus", menus);

                    pageContents.Add(pageContent);
                }
            }

            return new ListResult
            {
                PageContents = pageContents,
                Total = total,
                PageSize = site.PageSize
            };
        }

        public enum SearchType
        {
            All,
            Admin,
            User
        }

        public class ListRequest : SiteRequest
        {
            public SearchType SearchType { get; set; }
            public List<int> ChannelIds { get; set; }
            public bool IsAllContents { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public IEnumerable<KeyValuePair<string, string>> Items { get; set; }
            public int Page { get; set; }
            public bool IsAdvanced { get; set; }
            public bool IsCheckedLevels { get; set; }
            public List<int> CheckedLevels { get; set; }
            public bool IsTop { get; set; }
            public bool IsRecommend { get; set; }
            public bool IsHot { get; set; }
            public bool IsColor { get; set; }
            public List<string> GroupNames { get; set; }
            public List<string> TagNames { get; set; }
        }

        public class ListResult
        {
            public List<Content> PageContents { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
        }
    }
}
