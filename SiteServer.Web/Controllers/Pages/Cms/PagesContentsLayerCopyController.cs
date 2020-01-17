using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/contentsLayerCopy")]
    public class PagesContentsLayerCopyController : ApiController
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
                var channelContentIds =
                    ChannelContentId.ParseMinContentInfoList(request.GetQueryString("channelContentIds"));

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents) ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentTranslate))
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
                    dict["checkState"] =
                        CheckManager.GetCheckState(site, contentInfo);
                    retVal.Add(dict);
                }

                var sites = new List<object>();
                var channels = new List<object>();

                var siteIdList = await request.AdminPermissionsImpl.GetSiteIdListAsync();
                foreach (var permissionSiteId in siteIdList)
                {
                    var permissionSite = await DataProvider.SiteRepository.GetAsync(permissionSiteId);
                    sites.Add(new
                    {
                        permissionSite.Id,
                        permissionSite.SiteName
                    });
                }

                var channelIdList = await request.AdminPermissionsImpl.GetChannelIdListAsync(site.Id,
                    Constants.ChannelPermissions.ContentAdd);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = await ChannelManager.GetChannelAsync(site.Id, permissionChannelId);
                    channels.Add(new
                    {
                        permissionChannelInfo.Id,
                        ChannelName = await ChannelManager.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
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

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents))
                {
                    return Unauthorized();
                }

                var channels = new List<object>();
                var channelIdList = await request.AdminPermissionsImpl.GetChannelIdListAsync(siteId,
                    Constants.ChannelPermissions.ContentAdd);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = await ChannelManager.GetChannelAsync(siteId, permissionChannelId);
                    channels.Add(new
                    {
                        permissionChannelInfo.Id,
                        ChannelName = await ChannelManager.GetChannelNameNavigationAsync(siteId, permissionChannelId)
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
                var channelContentIds =
                    ChannelContentId.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var targetSiteId = request.GetPostInt("targetSiteId");
                var targetChannelId = request.GetPostInt("targetChannelId");
                var copyType = TranslateUtils.ToEnum(request.GetPostString("copyType"), TranslateContentType.Copy);

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents) ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentTranslate))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                foreach (var channelContentId in channelContentIds)
                {
                    await ContentUtility.TranslateAsync(site, channelContentId.ChannelId, channelContentId.Id, targetSiteId, targetChannelId, copyType);
                }

                await request.AddSiteLogAsync(siteId, channelId, "复制内容", string.Empty);

                await CreateManager.TriggerContentChangedEventAsync(siteId, channelId);

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
