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
    
    [RoutePrefix("home/contentsLayerCut")]
    public class HomeContentsLayerCutController : ApiController
    {
        private const string Route = "";
        private const string RouteGetChannels = "actions/getChannels";

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
                        Constants.ChannelPermissions.ContentTranslate))
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

                var sites = new List<object>();
                var channels = new List<object>();

                var siteIdList = await request.UserPermissionsImpl.GetSiteIdListAsync();
                foreach (var permissionSiteId in siteIdList)
                {
                    var permissionSite = await DataProvider.SiteRepository.GetAsync(permissionSiteId);
                    sites.Add(new
                    {
                        permissionSite.Id,
                        permissionSite.SiteName
                    });
                }

                var channelIdList = await request.UserPermissionsImpl.GetChannelIdListAsync(site.Id,
                    Constants.ChannelPermissions.ContentAdd);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = await DataProvider.ChannelRepository.GetAsync(permissionChannelId);
                    channels.Add(new
                    {
                        permissionChannelInfo.Id,
                        ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
                    });
                }

                return Ok(new
                {
                    Value = retVal,
                    Sites = sites,
                    Channels = channels,
                    Site = site
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteGetChannels)]
        public async Task<IHttpActionResult> GetChannels()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");

                var channels = new List<object>();
                var channelIdList = await request.UserPermissionsImpl.GetChannelIdListAsync(siteId,
                    Constants.ChannelPermissions.ContentAdd);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = await DataProvider.ChannelRepository.GetAsync(permissionChannelId);
                    channels.Add(new
                    {
                        permissionChannelInfo.Id,
                        ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(siteId, permissionChannelId)
                    });
                }

                return Ok(new
                {
                    Value = channels
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
                var targetSiteId = request.GetPostInt("targetSiteId");
                var targetChannelId = request.GetPostInt("targetChannelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentTranslate))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                foreach (var contentId in contentIdList)
                {
                    await ContentUtility.TranslateAsync(site, channelId, contentId, targetSiteId, targetChannelId, TranslateContentType.Cut);
                }

                await request.AddSiteLogAsync(siteId, channelId, "转移内容", string.Empty);

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
