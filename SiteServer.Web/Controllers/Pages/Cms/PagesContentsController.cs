using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
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
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var page = request.GetQueryInt("page");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = channelInfo.Additional.IsSelfOnly
                    ? request.AdminId
                    : request.AdminPermissionsImpl.GetAdminId(siteId, channelId);
                var isAllContents = channelInfo.Additional.IsAllContents;

                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var columns = ContentManager.GetContentColumns(site, channelInfo, false);

                var pageContentInfoList = new List<ContentInfo>();
                var count = ContentManager.GetCount(site, channelInfo, adminId, isAllContents);
                var pages = Convert.ToInt32(Math.Ceiling((double)count / site.Additional.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    var offset = site.Additional.PageSize * (page - 1);
                    var limit = site.Additional.PageSize;

                    var pageContentIds = ContentManager.GetChannelContentIdList(site, channelInfo, adminId, isAllContents, offset, limit);

                    var sequence = offset + 1;
                    foreach (var channelContentId in pageContentIds)
                    {
                        var contentInfo = ContentManager.GetContentInfo(site, channelContentId.ChannelId, channelContentId.ContentId);
                        if (contentInfo == null) continue;

                        var menus = PluginMenuManager.GetContentMenus(pluginIds, contentInfo);
                        contentInfo.Set("PluginMenus", menus);

                        var channelName = ChannelManager.GetChannelNameNavigation(siteId, channelId, channelContentId.ChannelId);
                        contentInfo.Set("ChannelName", channelName);

                        pageContentInfoList.Add(await ContentManager.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
                    }
                }

                var permissions = new
                {
                    IsAdd = request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.Additional.IsContentAddable,
                    IsDelete = request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentDelete),
                    IsEdit = request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit),
                    IsTranslate = request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate),
                    IsCheck = request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck),
                    IsCreate = request.AdminPermissionsImpl.HasSitePermissions(site.Id, ConfigManager.WebSitePermissions.Create) || request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.CreatePage),
                    IsChannelEdit = request.AdminPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit)
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
        public async Task<IHttpActionResult> Create()
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

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

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
