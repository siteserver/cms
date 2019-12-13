using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
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
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var page = request.GetQueryInt("page");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents) ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var adminId = channelInfo.IsSelfOnly
                    ? request.AdminId
                    : await request.AdminPermissionsImpl.GetAdminIdAsync(siteId, channelId);
                var isAllContents = channelInfo.IsAllContents;

                var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
                var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

                var columns = await DataProvider.ContentRepository.GetContentColumnsAsync(site, channelInfo, false);

                var pageContentInfoList = new List<Content>();
                var ccIds = await DataProvider.ContentRepository.GetChannelContentIdListAsync(site, channelInfo,
                    adminId, isAllContents);
                var count = ccIds.Count();
                var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
                if (pages == 0) pages = 1;

                if (count > 0)
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

                var permissions = new
                {
                    IsAdd = await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentAdd) && channelInfo.IsContentAddable,
                    IsDelete = await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentDelete),
                    IsEdit = await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentEdit),
                    IsTranslate = await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentTranslate),
                    IsCheck = await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                    IsCreate = await request.AdminPermissionsImpl.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.CreatePage),
                    IsChannelEdit =await request.AdminPermissionsImpl.HasChannelPermissionsAsync(site.Id, channelInfo.Id, Constants.ChannelPermissions.ChannelEdit)
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
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
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
