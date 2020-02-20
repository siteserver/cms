using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsRecycleController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ListResult> List([FromBody] ListRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ContentsRecycle))
            {
                return Request.Unauthorized<ListResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ListResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.RecycleContents);

            var pageContents = new List<Content>();
            var (total, pageSummaries) = await DataProvider.ContentRepository.RecycleSearch(site, request.Page, request.ChannelId, request.StartDate, request.EndDate, request.Items, request.IsCheckedLevels, request.CheckedLevels, request.IsTop, request.IsRecommend, request.IsHot, request.IsColor, request.GroupNames, request.TagNames);

            if (total > 0)
            {
                var offset = site.PageSize * (request.Page - 1);

                var sequence = offset + 1;
                foreach (var summary in pageSummaries)
                {
                    var content = await DataProvider.ContentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                    if (content == null) continue;

                    var pageContent =
                        await ColumnsManager.CalculateContentListAsync(sequence++, site, request.SiteId, content, columns, null);

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

        public class ListRequest : SiteRequest
        {
            public int? ChannelId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public IEnumerable<KeyValuePair<string, string>> Items { get; set; }
            public int Page { get; set; }
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
