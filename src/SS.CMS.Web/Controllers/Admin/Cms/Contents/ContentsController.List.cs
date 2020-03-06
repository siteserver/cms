using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<ListResult>> List([FromBody] ListRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentView,
                    Constants.ChannelPermissions.ContentAdd,
                    Constants.ChannelPermissions.ContentEdit,
                    Constants.ChannelPermissions.ContentDelete,
                    Constants.ChannelPermissions.ContentTranslate,
                    Constants.ChannelPermissions.ContentArrange,
                    Constants.ChannelPermissions.ContentCheckLevel1,
                    Constants.ChannelPermissions.ContentCheckLevel2,
                    Constants.ChannelPermissions.ContentCheckLevel3,
                    Constants.ChannelPermissions.ContentCheckLevel4,
                    Constants.ChannelPermissions.ContentCheckLevel5))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = await _pluginManager.GetContentColumnsAsync(pluginIds);

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            var pageContents = new List<Content>();
            List<ContentSummary> summaries;
            if (!string.IsNullOrEmpty(request.SearchType) &&
                !string.IsNullOrEmpty(request.SearchText) ||
                request.IsAdvanced)
            {
                summaries = await _contentRepository.Search(site, channel, channel.IsAllContents, request.SearchType, request.SearchText, request.IsAdvanced, request.CheckedLevels, request.IsTop, request.IsRecommend, request.IsHot, request.IsColor, request.GroupNames, request.TagNames);
            }
            else
            {
                summaries = await _contentRepository.GetSummariesAsync(site, channel, channel.IsAllContents);
            }
            var total = summaries.Count;

            if (total > 0)
            {
                var offset = site.PageSize * (request.Page - 1);
                var pageSummaries = summaries.Skip(offset).Take(site.PageSize).ToList();

                var sequence = offset + 1;
                foreach (var summary in pageSummaries)
                {
                    var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                    if (content == null) continue;

                    var pageContent =
                        await columnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, content, columns, pluginColumns);

                    var menus = await _pluginManager.GetContentMenusAsync(pluginIds, pageContent);
                    pageContent.Set("PluginMenus", menus);

                    pageContents.Add(pageContent);
                }
            }

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissions, site, request.ChannelId);
            var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true));

            var permissions = new Permissions
            {
                IsAdd = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentAdd),
                IsDelete = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentDelete),
                IsEdit = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentEdit),
                IsArrange = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentArrange),
                IsTranslate = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentTranslate),
                IsCheck = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                IsCreate = await auth.AdminPermissions.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.CreatePage),
                IsChannelEdit = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ChannelEdit)
            };

            return new ListResult
            {
                PageContents = pageContents,
                Total = total,
                PageSize = site.PageSize,
                Columns = columns,
                IsAllContents = channel.IsAllContents,
                CheckedLevels = checkedLevels,
                Permissions = permissions
            };
        }

        public class ListRequest : ChannelRequest
        {
            public int Page { get; set; }
            public string SearchType { get; set; }
            public string SearchText { get; set; }
            public bool IsAdvanced { get; set; }
            public List<int> CheckedLevels { get; set; }
            public bool IsTop { get; set; }
            public bool IsRecommend { get; set; }
            public bool IsHot { get; set; }
            public bool IsColor { get; set; }
            public List<string> GroupNames { get; set; }
            public List<string> TagNames { get; set; }
        }

        public class Permissions
        {
            public bool IsAdd { get; set; }
            public bool IsDelete { get; set; }
            public bool IsEdit { get; set; }
            public bool IsArrange { get; set; }
            public bool IsTranslate { get; set; }
            public bool IsCheck { get; set; }
            public bool IsCreate { get; set; }
            public bool IsChannelEdit { get; set; }
        }

        public class ListResult
        {
            public List<Content> PageContents { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public bool IsAllContents { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public Permissions Permissions { get; set; }
        }
    }
}
