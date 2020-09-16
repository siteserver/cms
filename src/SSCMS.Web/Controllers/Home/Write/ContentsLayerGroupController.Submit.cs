using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerGroupController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            if (request.PageType == "setGroup")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
                    if (contentInfo == null) continue;

                    var list = contentInfo.GroupNames;
                    foreach (var name in request.GroupNames)
                    {
                        if (!list.Contains(name)) list.Add(name);
                    }
                    contentInfo.GroupNames = list;

                    await _contentRepository.UpdateAsync(site, channel, contentInfo);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{ListUtils.ToString(request.GroupNames)}");
            }
            else if (request.PageType == "cancelGroup")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
                    if (contentInfo == null) continue;

                    var list = contentInfo.GroupNames;
                    foreach (var name in request.GroupNames)
                    {
                        if (list.Contains(name)) list.Remove(name);
                    }
                    contentInfo.GroupNames = list;

                    await _contentRepository.UpdateAsync(site, channel, contentInfo);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "批量取消内容组", $"内容组:{ListUtils.ToString(request.GroupNames)}");
            }
            else if (request.PageType == "addGroup")
            {
                var groupInfo = new ContentGroup
                {
                    GroupName = AttackUtils.FilterXss(request.GroupName),
                    SiteId = request.SiteId,
                    Description = AttackUtils.FilterXss(request.Description)
                };

                if (await _contentGroupRepository.IsExistsAsync(request.SiteId, groupInfo.GroupName))
                {
                    await _contentGroupRepository.UpdateAsync(groupInfo);
                    await _authManager.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{groupInfo.GroupName}");
                }
                else
                {
                    await _contentGroupRepository.InsertAsync(groupInfo);
                    await _authManager.AddSiteLogAsync(request.SiteId, "添加内容组", $"内容组:{groupInfo.GroupName}");
                }

                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
                    if (contentInfo == null) continue;

                    var list = contentInfo.GroupNames;
                    if (!list.Contains(groupInfo.GroupName)) list.Add(groupInfo.GroupName);
                    contentInfo.GroupNames = list;

                    await _contentRepository.UpdateAsync(site, channel, contentInfo);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{groupInfo.GroupName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
