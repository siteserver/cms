using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree([FromBody]TreeRequest request)
        {
            var siteIdList = await _authManager.GetSiteIdsAsync();
            if (siteIdList == null || siteIdList.Count == 0)
            {
                return Unauthorized();
            }

            var sites = new List<Select<int>>();
            Site site = null;
            foreach (var siteId in siteIdList)
            {
                var permissionSite = await _siteRepository.GetAsync(siteId);
                if (request.SiteId == siteId)
                {
                    site = permissionSite;
                }
                sites.Add(new Select<int>
                {
                    Value = permissionSite.Id,
                    Label = permissionSite.SiteName
                });
            }

            if (site == null && siteIdList.Count > 0)
            {
                site = await _siteRepository.GetAsync(siteIdList[0]);
            }

            if (site == null) return Unauthorized();

            var channel = await _channelRepository.GetAsync(site.Id);
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
                var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
                var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);
                var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

                var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
                var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);
                var permissions = new Permissions
                {
                    IsAdd = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Add),
                    IsDelete = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Delete),
                    IsEdit = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Edit),
                    IsArrange = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Arrange),
                    IsTranslate = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Translate),
                    IsCheck = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.CheckLevel1),
                    IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, AuthTypes.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.ContentPermissions.Create),
                    IsChannelEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.ChannelPermissions.Edit)
                };

                return new TreeResult
                {
                    Sites = sites,
                    SiteId = site.Id,
                    SiteName = site.SiteName,
                    SiteUrl = siteUrl,
                    Root = root,
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
            public List<Select<int>> Sites { get; set; }
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string SiteUrl { get; set; }
            public Cascade<int> Root { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public Permissions Permissions { get; set; }
        }
    }
}
