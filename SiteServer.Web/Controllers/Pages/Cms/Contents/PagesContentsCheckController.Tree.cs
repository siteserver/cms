using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsCheckController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<TreeResult> Tree([FromBody]SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ContentsCheck))
            {
                return Request.Unauthorized<TreeResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<TreeResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var root = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, true);

            var siteUrl = await PageUtility.GetSiteUrlAsync(site, true);
            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);
            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);
            var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

            var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.CheckContents);

            return new TreeResult
            {
                Root = root,
                SiteUrl = siteUrl,
                GroupNames = groupNames,
                TagNames = tagNames,
                CheckedLevels = checkedLevels,
                Columns = columns
            };
        }

        public class TreeResult
        {
            public Cascade<int> Root { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public List<ContentColumn> Columns { get; set; }
        }
    }
}
