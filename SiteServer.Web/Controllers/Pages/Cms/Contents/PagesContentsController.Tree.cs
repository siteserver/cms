using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<TreeResult> Tree([FromBody]TreeRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents))
            {
                return Request.Unauthorized<TreeResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<TreeResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var root = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel);

            if (!request.Reload)
            {
                var siteUrl = await PageUtility.GetSiteUrlAsync(site, true);
                var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);
                var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);
                var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

                return new TreeResult
                {
                    Root = root,
                    SiteUrl = siteUrl,
                    GroupNames = groupNames,
                    TagNames = tagNames,
                    CheckedLevels = checkedLevels
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

        public class TreeResult
        {
            public Cascade<int> Root { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
        }
    }
}
