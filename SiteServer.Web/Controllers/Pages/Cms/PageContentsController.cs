using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("api/pages/cms/contents")]
    public class PageContentsController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var searchType = request.GetQueryString("searchType");
                var searchWord = request.GetQueryString("searchWord");
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

                var offset = siteInfo.Additional.PageSize * (page - 1);
                var limit = siteInfo.Additional.PageSize;

                var pageContents = ContentManager.GetContentInfoList(siteInfo, channelInfo, offset, limit);
                var retval = new List<Dictionary<string, object>>();
                foreach (var contentInfo in pageContents)
                {
                    var dict = contentInfo.ToDictionary();
                    //dict["title"] = WebUtils.GetContentTitle(siteInfo, contentInfo, string.Empty);
                    dict["title"] = ContentUtility.FormatTitle(contentInfo.GetString(BackgroundContentAttribute.TitleFormatString), contentInfo.Title);
                    dict["checkState"] = CheckManager.GetCheckState(siteInfo, contentInfo.IsChecked, contentInfo.CheckedLevel);
                    retval.Add(dict);
                }

                object permissions = null;
                if (page == 1)
                {
                    permissions = new
                    {
                        IsAdd = request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.Additional.IsContentAddable,
                        IsDelete = request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentDelete),
                        IsEdit = request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit),
                        IsTranslate = request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate),
                        IsCheck = request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck),
                        IsCreate = request.AdminPermissions.HasSitePermissions(siteInfo.Id, ConfigManager.WebSitePermissions.Create) || request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.CreatePage),
                        IsChannelEdit = request.AdminPermissions.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit)
                    };
                }

                var attributes = ChannelManager.GetContentAttributesToList(siteInfo, channelInfo, false);
                var pages = Convert.ToInt32(
                    Math.Ceiling(Convert.ToDouble(channelInfo.ContentNum / siteInfo.Additional.PageSize)));
                if (pages == 0) pages = 1;

                return Ok(new
                {
                    Value = retval,
                    Count = channelInfo.ContentNum,
                    Pages = pages,
                    Permissions = permissions,
                    Attributes = attributes
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteCreate)]
        public IHttpActionResult Create()
        {
            try
            {
                var request = new AuthRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetPostString("contentIds"));

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                foreach (var contentId in contentIdList)
                {
                    CreateManager.CreateContent(siteId, channelInfo.Id, contentId);
                }

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
