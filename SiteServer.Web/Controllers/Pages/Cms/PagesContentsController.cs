using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/contents")]
    public partial class PagesContentsController : ApiController
    {
        private const string Route = "";
        private const string RouteChannels = "channels";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public async Task<ListResult> List()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            var siteId = auth.GetQueryInt("siteId");
            var channelId = auth.GetQueryInt("channelId");
            var page = auth.GetQueryInt("page");

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentView,
                    Constants.ChannelPermissions.ContentAdd,
                    Constants.ChannelPermissions.ContentEdit,
                    Constants.ChannelPermissions.ContentDelete,
                    Constants.ChannelPermissions.ContentTranslate,
                    Constants.ChannelPermissions.ContentArrange,
                    Constants.ChannelPermissions.ContentCheckLevel1,
                    Constants.ChannelPermissions.ContentCheckLevel2,
                    Constants.ChannelPermissions.ContentCheckLevel3,
                    Constants.ChannelPermissions.ContentCheckLevel4,
                    Constants.ChannelPermissions.ContentCheckLevel5))
            {
                return Request.Unauthorized<ListResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return Request.BadRequest<ListResult>("无法确定内容对应的站点");

            var channel = await ChannelManager.GetChannelAsync(siteId, channelId);
            if (channel == null) return Request.BadRequest<ListResult>("无法确定内容对应的栏目");

            var adminId = channel.IsSelfOnly
                ? auth.AdminId
                : await auth.AdminPermissionsImpl.GetAdminIdAsync(siteId, channelId);
            var isAllContents = channel.IsAllContents;

            var pluginIds = PluginContentManager.GetContentPluginIds(channel);
            var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

            var columns = await DataProvider.ContentRepository.GetContentColumnsAsync(site, channel, false);

            var pageContentInfoList = new List<Content>();
            var ccIds = await DataProvider.ContentRepository.GetChannelContentIdListAsync(site, channel,
                adminId, isAllContents);
            var total = ccIds.Count();

            if (total > 0)
            {
                var offset = site.PageSize * (page - 1);
                var limit = site.PageSize;
                var pageCcIds = ccIds.Skip(offset).Take(limit).ToList();

                var sequence = offset + 1;
                foreach (var channelContentId in pageCcIds)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
                    if (contentInfo == null) continue;

                    var menus = await PluginMenuManager.GetContentMenusAsync(pluginIds, contentInfo);
                    contentInfo.Set("PluginMenus", menus);

                    var channelName = await ChannelManager.GetChannelNameNavigationAsync(siteId, channelId, channelContentId.ChannelId);
                    contentInfo.Set("ChannelName", channelName);

                    pageContentInfoList.Add(await DataProvider.ContentRepository.CalculateAsync(sequence++, contentInfo, columns, pluginColumns));
                }
            }

            var permissions = new Permissions
            {
                IsAdd = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentAdd),
                IsDelete = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentDelete),
                IsEdit = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentEdit),
                IsArrange = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentArrange),
                IsTranslate = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentTranslate),
                IsCheck = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                IsCreate = await auth.AdminPermissionsImpl.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.CreatePage),
                IsChannelEdit = await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ChannelEdit)
            };

            return new ListResult
            {
                PageContents = pageContentInfoList,
                Total = total,
                PageSize = site.PageSize,
                Permissions = permissions,
                Columns = columns,
                IsAllContents = isAllContents
            };
        }

        [HttpGet, Route(RouteChannels)]
        public async Task<Cascade<int>> GetChannels([FromUri]SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents))
            {
                return Request.Unauthorized<Cascade<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<Cascade<int>>("无法确定内容对应的站点");

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.SiteId);
            return await ChannelManager.GetCascadeAsync(site, channel, 0);
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<IHttpActionResult> Create()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetPostInt("siteId");
                var channelContentIds = request.GetPostObject<List<ChannelContentId>>("channelContentIds");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                foreach (var channelContentId in channelContentIds)
                {
                    await CreateManager.CreateContentAsync(siteId, channelContentId.ChannelId, channelContentId.Id);
                }

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
