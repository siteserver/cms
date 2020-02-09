using System;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerGroup")]
    public class HomeContentsLayerGroupController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var contentGroupNameList = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(siteId);

                return Ok(new
                {
                    Value = contentGroupNameList
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = Utilities.GetIntList(request.GetPostString("contentIds"));
                var pageType = request.GetPostString("pageType");
                var groupNames = Utilities.GetStringList(request.GetPostString("groupNames"));
                var groupName = request.GetPostString("groupName");
                var description = request.GetPostString("description");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (pageType == "setGroup")
                {
                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        var list = contentInfo.GroupNames;
                        foreach (var name in groupNames)
                        {
                            if (!list.Contains(name)) list.Add(name);
                        }
                        contentInfo.GroupNames = list;

                        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "批量设置内容组", $"内容组:{Utilities.ToString(groupNames)}");
                }
                else if(pageType == "cancelGroup")
                {
                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        var list = contentInfo.GroupNames;
                        foreach (var name in groupNames)
                        {
                            if (list.Contains(name)) list.Remove(name);
                        }
                        contentInfo.GroupNames = list;

                        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "批量取消内容组", $"内容组:{Utilities.ToString(groupNames)}");
                }
                else if (pageType == "addGroup")
                {
                    var groupInfo = new ContentGroup
                    {
                        GroupName = AttackUtils.FilterXss(groupName),
                        SiteId = siteId,
                        Description = AttackUtils.FilterXss(description)
                    };

                    if (await DataProvider.ContentGroupRepository.IsExistsAsync(siteId, groupInfo.GroupName))
                    {
                        await DataProvider.ContentGroupRepository.UpdateAsync(groupInfo);
                        await request.AddSiteLogAsync(siteId, "修改内容组", $"内容组:{groupInfo.GroupName}");
                    }
                    else
                    {
                        await DataProvider.ContentGroupRepository.InsertAsync(groupInfo);
                        await request.AddSiteLogAsync(siteId, "添加内容组", $"内容组:{groupInfo.GroupName}");
                    }

                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        var list = contentInfo.GroupNames;
                        if (!list.Contains(groupInfo.GroupName)) list.Add(groupInfo.GroupName);
                        contentInfo.GroupNames = list;

                        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "批量设置内容组", $"内容组:{groupInfo.GroupName}");
                }

                return Ok(new
                {
                    Value = contentIdList
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
