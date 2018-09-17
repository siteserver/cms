using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("api/pages/cms/contentsLayerView")]
    public class PageContentsLayerViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                if (contentInfo == null) return BadRequest("无法确定对应的内容");

                var channelName = ChannelManager.GetChannelNameNavigation(siteId, channelId);

                var dict = contentInfo.ToDictionary();
                dict["title"] = ContentUtility.FormatTitle(contentInfo.GetString(BackgroundContentAttribute.TitleFormatString), contentInfo.Title);
                dict["checkState"] = CheckManager.GetCheckState(siteInfo, contentInfo.IsChecked, contentInfo.CheckedLevel);

                var attributes = ChannelManager.GetContentAttributesToList(siteInfo, channelInfo, true);

                return Ok(new
                {
                    Value = dict,
                    ChannelName = channelName,
                    Attributes = attributes
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
