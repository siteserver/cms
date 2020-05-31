using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
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
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var page = request.GetQueryInt("page");
                var type = request.GetQueryString("type");
                var keyword = request.GetQueryString("keyword");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView, 
                        ConfigManager.ChannelPermissions.ContentAdd,
                        ConfigManager.ChannelPermissions.ContentEdit,
                        ConfigManager.ChannelPermissions.ContentDelete,
                        ConfigManager.ChannelPermissions.ContentTranslate,
                        ConfigManager.ChannelPermissions.ContentArrange,
                        ConfigManager.ChannelPermissions.ContentCheck,
                        ConfigManager.ChannelPermissions.ContentCheckLevel1,
                        ConfigManager.ChannelPermissions.ContentCheckLevel2,
                        ConfigManager.ChannelPermissions.ContentCheckLevel3,
                        ConfigManager.ChannelPermissions.ContentCheckLevel4,
                        ConfigManager.ChannelPermissions.ContentCheckLevel5))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = channelInfo.Additional.IsSelfOnly
                    ? request.AdminId
                    : request.AdminPermissionsImpl.GetAdminId(siteId, channelId);
                var isAllContents = channelInfo.Additional.IsAllContents;

                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var columns = ContentManager.GetContentColumns(siteInfo, channelInfo, false);

                var pageContentInfoList = new List<ContentInfo>();
                //var ccIds = DataProvider.ContentDao.GetCacheChannelContentIdList(siteInfo, channelInfo, adminId, isAllContents, type, keyword);
                var ccIds = ContentManager.GetChannelContentIdList(siteInfo, channelInfo, adminId, isAllContents);
                var count = ccIds.Count;
                var pages = Convert.ToInt32(Math.Ceiling((double)count / siteInfo.Additional.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    var offset = siteInfo.Additional.PageSize * (page - 1);
                    var limit = siteInfo.Additional.PageSize;
                    var pageCcIds = ccIds.Skip(offset).Take(limit).ToList();

                    var sequence = offset + 1;
                    foreach (var (contentChannelId, contentId) in pageCcIds)
                    {
                        var contentInfo = ContentManager.GetContentInfo(siteInfo, contentChannelId, contentId);
                        if (contentInfo == null) continue;

                        var menus = PluginMenuManager.GetContentMenus(pluginIds, contentInfo);
                        contentInfo.Set("PluginMenus", menus);

                        var channelName = ChannelManager.GetChannelNameNavigation(siteId, channelId, contentChannelId);
                        contentInfo.Set("ChannelName", channelName);

                        pageContentInfoList.Add(ContentManager.Calculate(sequence++, contentInfo, columns, pluginColumns));
                    }
                }

                var permissions = new
                {
                    IsAdd = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.Additional.IsContentAddable,
                    IsDelete = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentDelete),
                    IsEdit = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit),
                    IsTranslate = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate),
                    IsArrange = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentArrange),
                    IsCheck = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck),
                    IsCreate = request.AdminPermissionsImpl.HasSitePermissions(siteInfo.Id, ConfigManager.SitePermissions.CreateContents) || request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.CreatePage),
                    IsChannelEdit = request.AdminPermissionsImpl.HasChannelPermissions(siteInfo.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit)
                };

                return Ok(new
                {
                    Value = pageContentInfoList,
                    Count = count,
                    Pages = pages,
                    Permissions = permissions,
                    Columns = columns,
                    IsAllContents = isAllContents
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
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var channelContentIds = request.GetPostObject<List<MinContentInfo>>("channelContentIds");

                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                foreach (var channelContentId in channelContentIds)
                {
                    CreateManager.CreateContent(siteId, channelContentId.ChannelId, channelContentId.Id);
                }

                return Ok(new
                {
                    Value = true
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
