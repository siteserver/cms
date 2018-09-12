using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("api/pages/cms/contents")]
    public class PageContentsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetChannelContents()
        {
            try
            {
                var request = new AuthRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var page = request.GetQueryInt("page");

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

                //var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                //var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(siteId, channelId);
                //var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
                //var attributes = TranslateUtils.StringCollectionToStringCollection(ChannelManager.GetContentAttributesOfDisplay(siteId, channelId));
                //var returnColumnNames = TranslateUtils.ObjectCollectionToString(TableColumnManager.GetTableColumnNameList(tableName, DataType.Text));

                var pageContents = ContentManager.GetContentInfoList(siteInfo, channelInfo, page);
                var count = channelInfo.ContentNum;

                var pages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(count / siteInfo.Additional.PageSize)));

                return Ok(new
                {
                    Value = pageContents,
                    Count = channelInfo.ContentNum,
                    Pages = pages
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
