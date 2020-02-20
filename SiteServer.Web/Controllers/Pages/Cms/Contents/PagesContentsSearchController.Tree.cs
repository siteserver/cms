using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsSearchController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<TreeResult> Tree([FromBody]TreeRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ContentsSearch))
            {
                return Request.Unauthorized<TreeResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<TreeResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
            var root = await DataProvider.ChannelRepository.GetCascadeCountAsync(site, channel, count);

            if (!request.Reload)
            {
                var siteUrl = await PageUtility.GetSiteUrlAsync(site, true);
                var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);
                var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);
                var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

                var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);
                var permissions = new Permissions
                {
                    IsAdd = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentAdd),
                    IsDelete = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentDelete),
                    IsEdit = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentEdit),
                    IsArrange = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentArrange),
                    IsTranslate = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentTranslate),
                    IsCheck = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                    IsCreate = await auth.AdminPermissionsImpl.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.CreatePage),
                    IsChannelEdit = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ChannelEdit)
                };

                return new TreeResult
                {
                    Root = root,
                    SiteUrl = siteUrl,
                    GroupNames = groupNames,
                    TagNames = tagNames,
                    CheckedLevels = checkedLevels,
                    Columns = columns,
                    Permissions = permissions
                };
            }

            return new TreeResult
            {
                Root = root
            };
        }

        public class TreeRequest : SiteRequest
        {
            public bool Reload { get; set; }
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

        public class TreeResult
        {
            public Cascade<int> Root { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public Permissions Permissions { get; set; }
        }
    }
}
