using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerDelete")]
    public class HomeContentsLayerDeleteController : ApiController
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
                var contentIdList = Utilities.GetIntList(request.GetQueryString("contentIds"));

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

                var retVal = new List<IDictionary<string, object>>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["checkState"] =
                        CheckManager.GetCheckState(site, contentInfo);
                    retVal.Add(dict);
                }

                return Ok(new
                {
                    Value = retVal
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
                var isRetainFiles = request.GetPostBool("isRetainFiles");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channel == null) return BadRequest("无法确定内容对应的栏目");

                if (!isRetainFiles)
                {
                    await DeleteManager.DeleteContentsAsync(site, channelId, contentIdList);
                }

                var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);

                if (contentIdList.Count == 1)
                {
                    var contentId = contentIdList[0];
                    var contentTitle = await DataProvider.ContentRepository.GetValueAsync(tableName, contentId, ContentAttribute.Title);
                    await request.AddSiteLogAsync(siteId, channelId, contentId, "删除内容",
                        $"栏目:{await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(siteId, channelId)},内容标题:{contentTitle}");
                }
                else
                {
                    await request.AddSiteLogAsync(siteId, "批量删除内容",
                        $"栏目:{await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(siteId, channelId)},内容条数:{contentIdList.Count}");
                }

                await DataProvider.ContentRepository.RecycleContentsAsync(site, channel, contentIdList);

                await CreateManager.TriggerContentChangedEventAsync(siteId, channelId);

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
