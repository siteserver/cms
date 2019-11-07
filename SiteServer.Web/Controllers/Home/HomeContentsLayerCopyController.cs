using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentsLayerCopy")]
    public class HomeContentsLayerCopyController : ApiController
    {
        private const string Route = "";
        private const string RouteGetChannels = "actions/getChannels";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetQueryString("contentIds"));

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentTranslate))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<Dictionary<string, object>>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["checkState"] =
                        CheckManager.GetCheckState(site, contentInfo);
                    retVal.Add(dict);
                }

                var sites = new List<object>();
                var channels = new List<object>();

                var siteIdList = request.UserPermissionsImpl.GetSiteIdList();
                foreach (var permissionSiteId in siteIdList)
                {
                    var permissionSite = await SiteManager.GetSiteAsync(permissionSiteId);
                    sites.Add(new
                    {
                        permissionSite.Id,
                        permissionSite.SiteName
                    });
                }

                var channelIdList = request.UserPermissionsImpl.GetChannelIdList(site.Id,
                    ConfigManager.ChannelPermissions.ContentAdd);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = ChannelManager.GetChannelInfo(site.Id, permissionChannelId);
                    channels.Add(new
                    {
                        permissionChannelInfo.Id,
                        ChannelName = ChannelManager.GetChannelNameNavigation(site.Id, permissionChannelId)
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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteGetChannels)]
        public IHttpActionResult GetChannels()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");

                var channels = new List<object>();
                var channelIdList = request.UserPermissionsImpl.GetChannelIdList(siteId,
                    ConfigManager.ChannelPermissions.ContentAdd);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = ChannelManager.GetChannelInfo(siteId, permissionChannelId);
                    channels.Add(new
                    {
                        permissionChannelInfo.Id,
                        ChannelName = ChannelManager.GetChannelNameNavigation(siteId, permissionChannelId)
                    });
                }

                return Ok(new
                {
                    Value = channels
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetPostString("contentIds"));
                var targetSiteId = request.GetPostInt("targetSiteId");
                var targetChannelId = request.GetPostInt("targetChannelId");
                var copyType = request.GetPostString("copyType");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentTranslate))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                foreach (var contentId in contentIdList)
                {
                    await ContentUtility.TranslateAsync(site, channelId, contentId, targetSiteId, targetChannelId, ETranslateContentTypeUtils.GetEnumType(copyType));
                }

                await request.AddSiteLogAsync(siteId, channelId, "复制内容", string.Empty);

                CreateManager.TriggerContentChangedEvent(siteId, channelId);

                return Ok(new
                {
                    Value = contentIdList
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
