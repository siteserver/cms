using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsSearchController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree([FromBody]TreeRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.ContentsSearch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            if (!request.Reload)
            {
                var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
                var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
                var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
                var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

                var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
                var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);
                var permissions = new Permissions
                {
                    IsAdd = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Add),
                    IsDelete = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Delete),
                    IsEdit = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Edit),
                    IsArrange = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Arrange),
                    IsTranslate = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Translate),
                    IsCheck = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.CheckLevel1),
                    IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, AuthTypes.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Create),
                    IsChannelEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteChannelPermissions.Edit)
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
