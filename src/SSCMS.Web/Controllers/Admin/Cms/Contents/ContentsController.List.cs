using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteList)]
        public async Task<ActionResult<ListResult>> List([FromBody] ListRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    AuthTypes.ContentPermissions.View,
                    AuthTypes.ContentPermissions.Add,
                    AuthTypes.ContentPermissions.Edit,
                    AuthTypes.ContentPermissions.Delete,
                    AuthTypes.ContentPermissions.Translate,
                    AuthTypes.ContentPermissions.Arrange,
                    AuthTypes.ContentPermissions.CheckLevel1,
                    AuthTypes.ContentPermissions.CheckLevel2,
                    AuthTypes.ContentPermissions.CheckLevel3,
                    AuthTypes.ContentPermissions.CheckLevel4,
                    AuthTypes.ContentPermissions.CheckLevel5))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            var pluginIds = _oldPluginManager.GetContentPluginIds(channel);
            var pluginColumns = _oldPluginManager.GetContentColumns(pluginIds);

            var columnsManager = new ColumnsManager(_databaseManager, _oldPluginManager, _pathManager);
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

            var allMenus = _settingsManager.GetMenus();
            //var contentPermissions = await _authManager.GetContentPermissionsAsync(request.SiteId, request.ChannelId);
            var contentMenus = allMenus.Where(x => ListUtils.ContainsIgnoreCase(x.Type, AuthTypes.Resources.Content)).ToList();
            //foreach (var appMenu in appMenus)
            //{
            //    appMenu.Children = GetChildren(appMenu, appPermissions);
            //}
            //menus.AddRange(appMenus);

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

                    var menus = await _oldPluginManager.GetContentMenusAsync(pluginIds, pageContent);
                    pageContent.Set("PluginMenus", menus);

                    pageContents.Add(pageContent);
                }
            }

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.ChannelId);
            var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true));

            var permissions = new Permissions
            {
                IsAdd = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Add),
                IsDelete = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Delete),
                IsEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Edit),
                IsArrange = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Arrange),
                IsTranslate = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Translate),
                IsCheck = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.CheckLevel1),
                IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, AuthTypes.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Create),
                IsChannelEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ChannelPermissions.Edit)
            };

            return new ListResult
            {
                PageContents = pageContents,
                Total = total,
                PageSize = site.PageSize,
                Columns = columns,
                IsAllContents = channel.IsAllContents,
                CheckedLevels = checkedLevels,
                Permissions = permissions,
                Menus = contentMenus
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
            public List<Menu> Menus { get; set; }
        }
    }
}
