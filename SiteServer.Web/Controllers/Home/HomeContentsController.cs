using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contents")]
    public class HomeContentsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> List()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var page = request.GetQueryInt("page");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = await request.AdminPermissionsImpl.GetAdminIdAsync(siteId, channelId);

                var columns = await DataProvider.ContentRepository.GetContentColumnsAsync(site, channelInfo, false);
                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

                var pageContentInfoList = new List<Content>();
                var count = await DataProvider.ContentRepository.GetCountAllAsync(site, channelInfo, adminId);

                var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    var offset = site.PageSize * (page - 1);
                    var limit = site.PageSize;

                    var pageContentIds = await DataProvider.ContentRepository.GetChannelContentIdListAsync(site, channelInfo, adminId, true, offset, limit);

                    var sequence = offset + 1;
                    foreach (var channelContentId in pageContentIds)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.ContentId);
                        if (contentInfo == null) continue;

                        var channelName = await ChannelManager.GetChannelNameNavigationAsync(siteId, channelId, channelContentId.ChannelId);
                        contentInfo.Set("ChannelName", channelName);

                        pageContentInfoList.Add(await DataProvider.ContentRepository.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
                    }
                }

                var permissions = new
                {
                    IsAdd = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentAdd) && channelInfo.IsContentAddable,
                    IsDelete = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentDelete),
                    IsEdit = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentEdit),
                    IsTranslate = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentTranslate),
                    IsCheck = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentCheck),
                    IsCreate = await request.UserPermissionsImpl.HasSitePermissionsAsync(site.Id, Constants.WebSitePermissions.Create) || await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.CreatePage),
                    IsChannelEdit = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ChannelEdit)
                };

                return Ok(new
                {
                    Value = pageContentInfoList,
                    Count = count,
                    Pages = pages,
                    Permissions = permissions,
                    Columns = columns
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
