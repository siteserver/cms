using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var siteIds = await _authManager.GetSiteIdsAsync();

            if (siteIds.Count == 0)
            {
                return new GetResult
                {
                    Unauthorized = true
                };
            }

            var sites = new List<Select<int>>();
            foreach (var siteId in siteIds)
            {
                var permissionSite = await _siteRepository.GetAsync(siteId);
                sites.Add(new Select<int>
                {
                    Value = permissionSite.Id,
                    Label = permissionSite.SiteName
                });
            }

            var site = await _siteRepository.GetAsync(siteIds[0]);

            var channel = await _channelRepository.GetAsync(site.Id);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);
            var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);
            var permissions = new GetPermissions
            {
                IsAdd = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Add),
                IsDelete = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Delete),
                IsEdit = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Edit),
                IsArrange = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Arrange),
                IsTranslate = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Translate),
                IsCheck = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.CheckLevel1),
                IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, MenuUtils.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, MenuUtils.ContentPermissions.Create),
                IsChannelEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, MenuUtils.ChannelPermissions.Edit)
            };

            return new GetResult
            {
                Unauthorized = false,
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
    }
}
