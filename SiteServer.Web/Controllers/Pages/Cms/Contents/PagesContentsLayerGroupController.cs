using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
    [RoutePrefix("pages/cms/contentsLayerGroup")]
    public class PagesContentsLayerGroupController : ApiController
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

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var contentGroupNameList = await DataProvider.ContentGroupDao.GetGroupNamesAsync(siteId);

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
                //var channelId = request.GetPostInt("channelId");
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var pageType = request.GetPostString("pageType");
                var groupNames = TranslateUtils.StringCollectionToStringList(request.GetPostString("groupNames"));
                var groupName = request.GetPostString("groupName");
                var description = request.GetPostString("description");

                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                if (pageType == "setGroup")
                {
                    foreach (var channelContentId in channelContentIds)
                    {
                        var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                        var contentInfo = await DataProvider.ContentDao.GetAsync(site, channelInfo, channelContentId.Id);
                        if (contentInfo == null) continue;

                        var list = TranslateUtils.StringCollectionToStringList(contentInfo.GroupNameCollection);
                        foreach (var name in groupNames)
                        {
                            if (!list.Contains(name)) list.Add(name);
                        }
                        contentInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);

                        await DataProvider.ContentDao.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "批量设置内容组", $"内容组:{TranslateUtils.ObjectCollectionToString(groupNames)}");
                }
                else if(pageType == "cancelGroup")
                {
                    foreach (var channelContentId in channelContentIds)
                    {
                        var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                        var contentInfo = await DataProvider.ContentDao.GetAsync(site, channelInfo, channelContentId.Id);
                        if (contentInfo == null) continue;

                        var list = TranslateUtils.StringCollectionToStringList(contentInfo.GroupNameCollection);
                        foreach (var name in groupNames)
                        {
                            if (list.Contains(name)) list.Remove(name);
                        }
                        contentInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);

                        await DataProvider.ContentDao.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "批量取消内容组", $"内容组:{TranslateUtils.ObjectCollectionToString(groupNames)}");
                }
                else if (pageType == "addGroup")
                {
                    var groupInfo = new ContentGroup
                    {
                        GroupName = AttackUtils.FilterXss(groupName),
                        SiteId = siteId,
                        Description = AttackUtils.FilterXss(description)
                    };

                    if (await DataProvider.ContentGroupDao.IsExistsAsync(siteId, groupInfo.GroupName))
                    {
                        await DataProvider.ContentGroupDao.UpdateAsync(groupInfo);
                        await request.AddSiteLogAsync(siteId, "修改内容组", $"内容组:{groupInfo.GroupName}");
                    }
                    else
                    {
                        await DataProvider.ContentGroupDao.InsertAsync(groupInfo);
                        await request.AddSiteLogAsync(siteId, "添加内容组", $"内容组:{groupInfo.GroupName}");
                    }

                    foreach (var channelContentId in channelContentIds)
                    {
                        var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                        var contentInfo = await DataProvider.ContentDao.GetAsync(site, channelInfo, channelContentId.Id);
                        if (contentInfo == null) continue;

                        var list = TranslateUtils.StringCollectionToStringList(contentInfo.GroupNameCollection);
                        if (!list.Contains(groupInfo.GroupName)) list.Add(groupInfo.GroupName);
                        contentInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);

                        await DataProvider.ContentDao.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "批量设置内容组", $"内容组:{groupInfo.GroupName}");
                }

                return Ok(new
                {
                    Value = true
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
