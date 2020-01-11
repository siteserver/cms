using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/contentsLayerDelete")]
    public class PagesContentsLayerDeleteController : ApiController
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
                var channelContentIds =
                    ChannelContentId.ParseMinContentInfoList(request.GetQueryString("channelContentIds"));

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<IDictionary<string, object>>();
                foreach (var channelContentId in channelContentIds)
                {
                    var contentChannelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, contentChannelInfo, channelContentId.Id);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["title"] = WebUtils.GetContentTitle(site, contentInfo, string.Empty);
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
                var channelContentIds =
                    ChannelContentId.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var isRetainFiles = request.GetPostBool("isRetainFiles");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents) ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!isRetainFiles)
                {
                    foreach (var channelContentId in channelContentIds)
                    {
                        await DeleteManager.DeleteContentAsync(site, channelContentId.ChannelId, channelContentId.Id);
                    }
                }

                var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

                if (channelContentIds.Count == 1)
                {
                    var channelContentId = channelContentIds[0];
                    var contentTitle = await DataProvider.ContentRepository.GetValueAsync(tableName, channelContentId.Id, ContentAttribute.Title);
                    await request.AddSiteLogAsync(siteId, channelContentId.ChannelId, channelContentId.Id, "删除内容",
                        $"栏目:{await ChannelManager.GetChannelNameNavigationAsync(siteId, channelContentId.ChannelId)},内容标题:{contentTitle}");
                }
                else
                {
                    await request.AddSiteLogAsync(siteId, "批量删除内容",
                        $"栏目:{await ChannelManager.GetChannelNameNavigationAsync(siteId, channelId)},内容条数:{channelContentIds.Count}");
                }

                foreach (var distinctChannelId in channelContentIds.Select(x => x.ChannelId).Distinct())
                {
                    var contentIdList = channelContentIds.Where(x => x.ChannelId == distinctChannelId)
                        .Select(x => x.Id).ToList();
                    await DataProvider.ContentRepository.UpdateTrashContentsAsync(siteId, distinctChannelId, tableName, contentIdList);

                    await CreateManager.TriggerContentChangedEventAsync(siteId, distinctChannelId);
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
