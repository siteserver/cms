using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerGroupController
    {
        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] AddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelContentIds = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var group = new ContentGroup
            {
                GroupName = request.GroupName,
                SiteId = request.SiteId,
                Description = request.Description
            };

            if (await _contentGroupRepository.IsExistsAsync(request.SiteId, group.GroupName))
            {
                await _contentGroupRepository.UpdateAsync(group);
                await _authManager.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{group.GroupName}");
            }
            else
            {
                await _contentGroupRepository.InsertAsync(group);
                await _authManager.AddSiteLogAsync(request.SiteId, "添加内容组", $"内容组:{group.GroupName}");
            }

            foreach (var channelContentId in channelContentIds)
            {
                var channel = await _channelRepository.GetAsync(channelContentId.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, channelContentId.Id);
                if (content == null) continue;

                var list = ListUtils.GetStringList(content.GroupNames);
                if (!list.Contains(group.GroupName)) list.Add(group.GroupName);
                content.GroupNames = list;

                await _contentRepository.UpdateAsync(site, channel, content);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{group.GroupName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}