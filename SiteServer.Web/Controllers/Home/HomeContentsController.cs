using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contents")]
    public class HomeContentsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> List()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var page = request.GetQueryInt("page");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = request.AdminPermissionsImpl.GetAdminId(siteId, channelId);

                var columns = ContentManager.GetContentColumns(site, channelInfo, false);
                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

                var pageContentInfoList = new List<ContentInfo>();
                var count = ContentManager.GetCount(site, channelInfo, adminId, true);

                var pages = Convert.ToInt32(Math.Ceiling((double)count / site.Additional.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
                {
                    var offset = site.Additional.PageSize * (page - 1);
                    var limit = site.Additional.PageSize;

                    var pageContentIds = ContentManager.GetChannelContentIdList(site, channelInfo, adminId, true, offset, limit);

                    var sequence = offset + 1;
                    foreach (var channelContentId in pageContentIds)
                    {
                        var contentInfo = ContentManager.GetContentInfo(site, channelContentId.ChannelId, channelContentId.ContentId);
                        if (contentInfo == null) continue;

                        var channelName = ChannelManager.GetChannelNameNavigation(siteId, channelId, channelContentId.ChannelId);
                        contentInfo.Set("ChannelName", channelName);

                        pageContentInfoList.Add(await ContentManager.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
                    }
                }

                var permissions = new
                {
                    IsAdd = request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && channelInfo.Additional.IsContentAddable,
                    IsDelete = request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentDelete),
                    IsEdit = request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentEdit),
                    IsTranslate = request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate),
                    IsCheck = request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ContentCheck),
                    IsCreate = request.UserPermissionsImpl.HasSitePermissions(site.Id, ConfigManager.WebSitePermissions.Create) || request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.CreatePage),
                    IsChannelEdit = request.UserPermissionsImpl.HasChannelPermissions(site.Id, channelInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit)
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
    }
}
