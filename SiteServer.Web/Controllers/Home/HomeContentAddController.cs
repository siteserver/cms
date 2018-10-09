using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("api/home/contentAdd")]
    public class HomeContentAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new RequestImpl();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return NotFound();

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return NotFound();

                var styles = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);

                ContentInfo contentInfo;

                if (contentId > 0)
                {
                    contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                }
                else
                {
                    contentInfo = new ContentInfo(new
                    {
                        SiteId = siteInfo.Id,
                        ChannelId = channelInfo.Id
                    });
                }

                return Ok(new
                {
                    Value = contentInfo,
                    styles
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
