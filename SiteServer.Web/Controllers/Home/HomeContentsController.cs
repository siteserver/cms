using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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

                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channel == null) return BadRequest("无法确定内容对应的栏目");

                var columns = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
                var pluginIds = PluginContentManager.GetContentPluginIds(channel);
                var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

                var pageContentInfoList = new List<Content>();
                var ccIds = await DataProvider.ContentRepository.GetSummariesAsync(site, channel, true);
                var count = ccIds.Count();

                var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    var offset = site.PageSize * (page - 1);
                    var limit = site.PageSize;
                    var pageCcIds = ccIds.Skip(offset).Take(limit).ToList();

                    var sequence = offset + 1;
                    foreach (var channelContentId in pageCcIds)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
                        if (contentInfo == null) continue;

                        pageContentInfoList.Add(await ColumnsManager.CalculateContentListAsync(sequence++, site, channelId, contentInfo, columns, pluginColumns));
                    }
                }

                var permissions = new
                {
                    IsAdd = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentAdd),
                    IsDelete = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentDelete),
                    IsEdit = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentEdit),
                    IsTranslate = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentTranslate),
                    IsCheck = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                    IsCreate = await request.UserPermissionsImpl.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.CreatePage),
                    IsChannelEdit = await request.UserPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ChannelEdit)
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
