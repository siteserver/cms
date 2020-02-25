using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerGroup")]
    public partial class ContentsLayerGroupController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public ContentsLayerGroupController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentDelete))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var contentGroupNameList = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);

            return new GetResult
            {
                GroupNames = contentGroupNameList
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            if (request.PageType == "setGroup")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
                    if (contentInfo == null) continue;

                    var list = contentInfo.GroupNames;
                    foreach (var name in request.GroupNames)
                    {
                        if (!list.Contains(name)) list.Add(name);
                    }
                    contentInfo.GroupNames = list;

                    await DataProvider.ContentRepository.UpdateAsync(site, channel, contentInfo);
                }

                await auth.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{Utilities.ToString(request.GroupNames)}");
            }
            else if (request.PageType == "cancelGroup")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
                    if (contentInfo == null) continue;

                    var list = contentInfo.GroupNames;
                    foreach (var name in request.GroupNames)
                    {
                        if (list.Contains(name)) list.Remove(name);
                    }
                    contentInfo.GroupNames = list;

                    await DataProvider.ContentRepository.UpdateAsync(site, channel, contentInfo);
                }

                await auth.AddSiteLogAsync(request.SiteId, "批量取消内容组", $"内容组:{Utilities.ToString(request.GroupNames)}");
            }
            else if (request.PageType == "addGroup")
            {
                var groupInfo = new ContentGroup
                {
                    GroupName = AttackUtils.FilterXss(request.GroupName),
                    SiteId = request.SiteId,
                    Description = AttackUtils.FilterXss(request.Description)
                };

                if (await DataProvider.ContentGroupRepository.IsExistsAsync(request.SiteId, groupInfo.GroupName))
                {
                    await DataProvider.ContentGroupRepository.UpdateAsync(groupInfo);
                    await auth.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{groupInfo.GroupName}");
                }
                else
                {
                    await DataProvider.ContentGroupRepository.InsertAsync(groupInfo);
                    await auth.AddSiteLogAsync(request.SiteId, "添加内容组", $"内容组:{groupInfo.GroupName}");
                }

                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
                    if (contentInfo == null) continue;

                    var list = contentInfo.GroupNames;
                    if (!list.Contains(groupInfo.GroupName)) list.Add(groupInfo.GroupName);
                    contentInfo.GroupNames = list;

                    await DataProvider.ContentRepository.UpdateAsync(site, channel, contentInfo);
                }

                await auth.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{groupInfo.GroupName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
