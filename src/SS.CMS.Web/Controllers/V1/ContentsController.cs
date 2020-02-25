using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.V1
{
    [Route("v1/contents")]
    public partial class ContentsController : ControllerBase
    {
        private const string Route = "";
        private const string RouteCheck = "check";
        private const string RouteChannel = "{siteId:int}/{channelId:int}";
        private const string RouteContent = "{siteId:int}/{channelId:int}/{id:int}";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public ContentsController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }

        [OpenApiOperation("添加内容API", "")]
        [HttpPost, Route(RouteChannel)]
        public async Task<ActionResult<Content>> Create([FromBody] Content request)
        {
            var auth = await _authManager.GetApiAsync();

            bool isAuth;
            if (request.SourceId == SourceManager.User)
            {
                isAuth = auth.IsUserLoggin && await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd);
            }
            else
            {
                isAuth = auth.IsApiAuthenticated && await
                             DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeContents) ||
                         auth.IsUserLoggin &&
                         await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                             Constants.ChannelPermissions.ContentAdd) ||
                         auth.IsAdminLoggin &&
                         await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                             Constants.ChannelPermissions.ContentAdd);
            }
            if (!isAuth) return Unauthorized();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var checkedLevel = request.CheckedLevel;

            var isChecked = checkedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                if (request.SourceId == SourceManager.User || auth.IsUserLoggin)
                {
                    isChecked = await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                        Constants.ChannelPermissions.ContentCheckLevel1);
                }
                else if (auth.IsAdminLoggin)
                {
                    isChecked = await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                        Constants.ChannelPermissions.ContentCheckLevel1);
                }
            }

            var contentInfo = new Content(request.ToDictionary())
            {
                SiteId = request.SiteId,
                ChannelId = request.ChannelId,
                AdminId = auth.AdminId,
                LastEditAdminId = auth.AdminId,
                UserId = auth.UserId,
                LastEditDate = DateTime.Now,
                SourceId = request.SourceId,
                Checked = isChecked,
                CheckedLevel = checkedLevel
            };

            contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(site, channel, contentInfo);

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentFormSubmit(new ContentFormSubmitEventArgs(request.SiteId, request.ChannelId, contentInfo.Id, request.ToDictionary(), contentInfo));
                }
                catch (Exception ex)
                {
                    await DataProvider.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(IPluginService.ContentFormSubmit));
                }
            }

            if (contentInfo.Checked)
            {
                await _createManager.CreateContentAsync(request.SiteId, request.ChannelId, contentInfo.Id);
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, contentInfo.Id, "添加内容",
                $"栏目:{await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(request.SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

            return contentInfo;
        }

        [OpenApiOperation("修改内容API", "")]
        [HttpPut, Route(RouteContent)]
        public async Task<ActionResult<Content>> Update([FromBody]Content request)
        {
            var auth = await _authManager.GetApiAsync();
            
            bool isAuth;
            if (request.SourceId == SourceManager.User)
            {
                isAuth = auth.IsUserLoggin && await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit);
            }
            else
            {
                isAuth = auth.IsApiAuthenticated && await
                             DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeContents) ||
                         auth.IsUserLoggin &&
                         await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                             Constants.ChannelPermissions.ContentEdit) ||
                         auth.IsAdminLoggin &&
                         await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                             Constants.ChannelPermissions.ContentEdit);
            }
            if (!isAuth) return Unauthorized();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var content = await DataProvider.ContentRepository.GetAsync(site, channelInfo, request.Id);
            if (content == null) return NotFound();

            content.LoadDict(request.ToDictionary());

            content.SiteId = request.SiteId;
            content.ChannelId = request.ChannelId;
            content.LastEditAdminId = auth.AdminId;
            content.LastEditDate = DateTime.Now;
            content.SourceId = request.SourceId;

            var postCheckedLevel = content.CheckedLevel;
            var isChecked = postCheckedLevel >= site.CheckContentLevel;
            var checkedLevel = postCheckedLevel;

            content.Checked = isChecked;
            content.CheckedLevel = checkedLevel;

            await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, content);

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentFormSubmit(new ContentFormSubmitEventArgs(request.SiteId, request.ChannelId, content.Id, content.ToDictionary(), content));
                }
                catch (Exception ex)
                {
                    await DataProvider.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(IPluginService.ContentFormSubmit));
                }
            }

            if (content.Checked)
            {
                await _createManager.CreateContentAsync(request.SiteId, request.ChannelId, content.Id);
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, content.Id, "修改内容",
                $"栏目:{await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(request.SiteId, content.ChannelId)},内容标题:{content.Title}");

            return content;
        }

        [OpenApiOperation("删除内容API", "")]
        [HttpDelete, Route(RouteContent)]
        public async Task<ActionResult<Content>> Delete(int siteId, int channelId, int id)
        {
            var auth = await _authManager.GetApiAsync();

            var isUserAuth = auth.IsUserLoggin && await auth.UserPermissions.HasChannelPermissionsAsync(siteId, channelId, Constants.ChannelPermissions.ContentDelete);
            var isApiAuth = auth.IsApiAuthenticated && await
                                DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeContents) ||
                            auth.IsUserLoggin &&
                            await auth.UserPermissions.HasChannelPermissionsAsync(siteId, channelId,
                                Constants.ChannelPermissions.ContentDelete) ||
                            auth.IsAdminLoggin &&
                            await auth.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                                Constants.ChannelPermissions.ContentDelete);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channel == null) return NotFound();

            if (!await auth.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                Constants.ChannelPermissions.ContentDelete)) return Unauthorized();

            var content = await DataProvider.ContentRepository.GetAsync(site, channel, id);
            if (content == null) return NotFound();

            await DataProvider.ContentRepository.DeleteAsync(site, channel, id);

            return content;
        }

        [OpenApiOperation("获取内容API", "")]
        [HttpGet, Route(RouteContent)]
        public async Task<ActionResult<Content>> Get(int siteId, int channelId, int id)
        {
            var auth = await _authManager.GetApiAsync();

            var isUserAuth = auth.IsUserLoggin && await auth.UserPermissions.HasChannelPermissionsAsync(siteId, channelId, Constants.ChannelPermissions.ContentView);
            var isApiAuth = auth.IsApiAuthenticated && await
                                DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeContents) ||
                            auth.IsUserLoggin &&
                            await auth.UserPermissions.HasChannelPermissionsAsync(siteId, channelId,
                                Constants.ChannelPermissions.ContentView) ||
                            auth.IsAdminLoggin &&
                            await auth.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                                Constants.ChannelPermissions.ContentView);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channelInfo == null) return NotFound();

            if (!await auth.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
                Constants.ChannelPermissions.ContentView)) return Unauthorized();

            var content = await DataProvider.ContentRepository.GetAsync(site, channelInfo, id);
            if (content == null) return NotFound();

            return content;
        }

        [OpenApiOperation("获取内容列表API", "")]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> GetContents([FromBody] QueryRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            var channelId = request.ChannelId ?? request.SiteId;

            var isUserAuth = auth.IsUserLoggin && await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, channelId, Constants.ChannelPermissions.ContentView);
            var isApiAuth = auth.IsApiAuthenticated && await
                                DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeContents) ||
                            auth.IsUserLoggin &&
                            await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, channelId,
                                Constants.ChannelPermissions.ContentView) ||
                            auth.IsAdminLoggin &&
                            await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, channelId,
                                Constants.ChannelPermissions.ContentView);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var tableName = site.TableName;
            var query = await GetQueryAsync(request.SiteId, request.ChannelId, request);
            var totalCount = await DataProvider.ContentRepository.GetCountAsync(tableName, query);
            var summaries = await DataProvider.ContentRepository.GetSummariesAsync(tableName, query.ForPage(request.Page, request.PerPage));

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var content = await DataProvider.ContentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                contents.Add(content);
            }

            return new QueryResult
            {
                Contents = contents,
                TotalCount = totalCount
            };
        }

        [OpenApiOperation("审核内容API", "")]
        [HttpPost, Route(RouteCheck)]
        public async Task<ActionResult<CheckResult>> CheckContents([FromBody] CheckRequest request)
        {
            var auth = await _authManager.GetApiAsync();

            if (!auth.IsApiAuthenticated ||
                !await DataProvider.AccessTokenRepository.IsScopeAsync(auth.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var contents = new List<Content>();
            foreach (var channelContentId in request.Contents)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(channelContentId.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, channelContentId.Id);
                if (content == null) continue;

                content.CheckAdminId = auth.AdminId;
                content.CheckDate = DateTime.Now;
                content.CheckReasons = request.Reasons;

                content.Checked = true;
                content.CheckedLevel = 0;

                await DataProvider.ContentRepository.UpdateAsync(site, channel, content);

                contents.Add(content);

                await DataProvider.ContentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = content.ChannelId,
                    ContentId = content.Id,
                    AdminId = auth.AdminId,
                    Checked = true,
                    CheckedLevel = 0,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });
            }

            await auth.AddSiteLogAsync(request.SiteId, "批量审核内容");

            foreach (var content in request.Contents)
            {
                await _createManager.CreateContentAsync(request.SiteId, content.ChannelId, content.Id);
            }

            foreach (var distinctChannelId in request.Contents.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await _createManager.CreateChannelAsync(request.SiteId, request.SiteId);

            return new CheckResult
            {
                Contents = contents
            };
        }

        //[OpenApiOperation("获取站点内容API", "")]
        //[HttpGet, Route(RouteSite)]
        //public async Task<IHttpActionResult> GetSiteContents(int siteId)
        //{
        //    try
        //    {
        //        var request = await AuthenticatedRequest.GetAuthAsync();
        //        var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
        //        bool isAuth;
        //        if (sourceId == SourceManager.User)
        //        {
        //            isAuth = request.IsUserLoggin && await request.UserPermissions.HasChannelPermissionsAsync(siteId, siteId, Constants.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated && await
        //                         DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     await request.UserPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                         Constants.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     await request.AdminPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                         Constants.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var site = await DataProvider.SiteRepository.GetAsync(siteId);
        //        if (site == null) return this.Error("无法确定内容对应的站点");

        //        if (!await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, siteId,
        //            Constants.ChannelPermissions.ContentView)) return Unauthorized();

        //        var tableName = site.TableName;

        //        var parameters = new ApiContentsParameters(request);

        //        var (channelContentIds, count) = await DataProvider.ContentRepository.GetChannelContentIdListBySiteIdAsync(tableName, siteId, parameters);
        //        var value = new List<IDictionary<string, object>>();
        //        foreach (var channelContentId in channelContentIds)
        //        {
        //            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
        //            if (contentInfo != null)
        //            {
        //                value.Add(contentInfo.ToDictionary());
        //            }
        //        }

        //        return new PageResponse(value, parameters.Top, parameters.Skip, request.HttpRequest.Url.AbsoluteUri) {Count = count});
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogUtils.AddErrorLogAsync(ex);
        //        return InternalServerError(ex);
        //    }
        //}

        //[OpenApiOperation("获取栏目内容API", "")]
        //[HttpGet, Route(RouteChannel)]
        //public async Task<IHttpActionResult> GetChannelContents(int siteId, int channelId)
        //{
        //    try
        //    {
        //        var request = await AuthenticatedRequest.GetAuthAsync();
        //        var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
        //        bool isAuth;
        //        if (sourceId == SourceManager.User)
        //        {
        //            isAuth = request.IsUserLoggin && await request.UserPermissions.HasChannelPermissionsAsync(siteId, channelId, Constants.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated && await
        //                         DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     await request.UserPermissions.HasChannelPermissionsAsync(siteId, channelId,
        //                         Constants.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
        //                         Constants.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var site = await DataProvider.SiteRepository.GetAsync(siteId);
        //        if (site == null) return this.Error("无法确定内容对应的站点");

        //        var channelInfo = await DataProvider.ChannelRepository.GetAsync(siteId, channelId);
        //        if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

        //        if (!await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
        //            Constants.ChannelPermissions.ContentView)) return Unauthorized();

        //        var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelInfo);

        //        var top = request.GetQueryInt("top", 20);
        //        var skip = request.GetQueryInt("skip");
        //        var like = request.GetQueryString("like");
        //        var orderBy = request.GetQueryString("orderBy");

        //        var (list, count) = await DataProvider.ContentRepository.ApiGetContentIdListByChannelIdAsync(tableName, siteId, channelId, top, skip, like, orderBy, request.QueryString);
        //        var value = new List<IDictionary<string, object>>();
        //        foreach(var (contentChannelId, contentId) in list)
        //        {
        //            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, contentChannelId, contentId);
        //            if (contentInfo != null)
        //            {
        //                value.Add(contentInfo.ToDictionary());
        //            }
        //        }

        //        return new PageResponse(value, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogUtils.AddErrorLogAsync(ex);
        //        return InternalServerError(ex);
        //    }
        //}

        //[OpenApiOperation("获取内容API", "")]
        //[HttpPost, Route(RouteSite)]
        //public async Task<QueryResult> GetSiteContents([FromUri]int siteId, [FromBody] QueryRequest request)
        //{
        //    var req = await AuthenticatedRequest.GetAuthAsync();
        //    var sourceId = req.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
        //    bool isAuth;
        //    if (sourceId == SourceManager.User)
        //    {
        //        isAuth = req.IsUserLoggin && await req.UserPermissions.HasChannelPermissionsAsync(siteId, siteId, Constants.ChannelPermissions.ContentView);
        //    }
        //    else
        //    {
        //        isAuth = req.IsApiAuthenticated && await
        //                     DataProvider.AccessTokenRepository.IsScopeAsync(req.ApiToken, Constants.ScopeContents) ||
        //                 req.IsUserLoggin &&
        //                 await req.UserPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                     Constants.ChannelPermissions.ContentView) ||
        //                 req.IsAdminLoggin &&
        //                 await req.AdminPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                     Constants.ChannelPermissions.ContentView);
        //    }
        //    if (!isAuth) return Request.Unauthorized<QueryResult>();

        //    var site = await DataProvider.SiteRepository.GetAsync(siteId);
        //    if (site == null) return Request.BadRequest<QueryResult>("无法确定内容对应的站点");

        //    if (!await req.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, siteId,
        //        Constants.ChannelPermissions.ContentView)) return Request.Unauthorized<QueryResult>();

        //    var tableName = site.TableName;
        //    var query = GetQuery(siteId, null, request);
        //    var totalCount = await DataProvider.ContentRepository.GetTotalCountAsync(tableName, query);
        //    var channelContentIds = await DataProvider.ContentRepository.GetChannelContentIdListAsync(tableName, query);

        //    var contents = new List<Content>();
        //    foreach (var channelContentId in channelContentIds)
        //    {
        //        var content = await DataProvider.ContentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
        //        contents.Add(content);
        //    }

        //    return new QueryResult
        //    {
        //        Contents = contents,
        //        TotalCount = totalCount
        //    };
        //}
    }
}
