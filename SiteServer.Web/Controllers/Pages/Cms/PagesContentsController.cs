using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contents")]
    public class PagesContentsController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetQueryInt("siteId");
                var channelId = rest.GetQueryInt("channelId");
                var page = rest.GetQueryInt("page");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var onlyAdminId = rest.AdminPermissionsImpl.GetOnlyAdminId(siteId, channelId);

                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var columns = ContentManager.GetContentColumns(siteInfo, channelInfo, false);

                var pageContentInfoList = new List<IDictionary<string, object>>();
                var count = ContentManager.GetCount(siteInfo, channelInfo, onlyAdminId);

                var pages = Convert.ToInt32(Math.Ceiling((double)count / siteInfo.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    var offset = siteInfo.PageSize * (page - 1);
                    var limit = siteInfo.PageSize;

                    var pageContentIds = ContentManager.GetContentIdList(siteInfo, channelInfo, onlyAdminId, offset, limit);

                    var sequence = offset + 1;
                    foreach (var contentId in pageContentIds)
                    {
                        var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        var menus = PluginMenuManager.GetContentMenus(pluginIds, contentInfo);
                        contentInfo.Set("PluginMenus", menus);

                        pageContentInfoList.Add(ContentManager.Calculate(sequence++, contentInfo, columns, pluginColumns));
                    }
                }

                var permissions = new
                {
                    IsAdd = rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.IsContentAddable,
                    IsDelete = rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentDelete),
                    IsEdit = rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit),
                    IsTranslate = rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate),
                    IsCheck = rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck),
                    IsCreate = rest.AdminPermissionsImpl.HasSitePermissions(siteInfo.Id, ConfigManager.WebSitePermissions.Create) || rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.CreatePage),
                    IsChannelEdit = rest.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit)
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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteCreate)]
        public IHttpActionResult Create()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(rest.GetPostString("contentIds"));

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
