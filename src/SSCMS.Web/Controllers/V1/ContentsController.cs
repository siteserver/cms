using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    /// <summary>
    /// 内容操作API
    /// </summary>
    [Authorize(Roles = AuthTypes.Roles.Api)]
    [Route(Constants.ApiV1Prefix)]
    public partial class ContentsController : ControllerBase
    {
        private const string Route = "contents";
        private const string RouteActionsCheck = "contents/actions/check";
        private const string RouteChannel = "contents/{siteId:int}/{channelId:int}";
        private const string RouteContent = "contents/{siteId:int}/{channelId:int}/{id:int}";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IOldPluginManager _oldPluginManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public ContentsController(IAuthManager authManager, ICreateManager createManager, IOldPluginManager oldPluginManager, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IErrorLogRepository errorLogRepository, IContentCheckRepository contentCheckRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _oldPluginManager = oldPluginManager;
            _accessTokenRepository = accessTokenRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _errorLogRepository = errorLogRepository;
            _contentCheckRepository = contentCheckRepository;
        }

        [OpenApiOperation("添加内容API", "")]
        [HttpPost, Route(RouteChannel)]
        public async Task<ActionResult<Content>> Create([FromBody] Content request)
        {
            bool isAuth;
            if (request.SourceId == SourceManager.User)
            {
                isAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add);
            }
            else
            {
                isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                         _authManager.IsUser &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add);
            }
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var checkedLevel = request.CheckedLevel;

            var isChecked = checkedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                if (request.SourceId == SourceManager.User || _authManager.IsUser)
                {
                    isChecked = await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.CheckLevel1);
                }
                else if (_authManager.IsAdmin)
                {
                    isChecked = await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.CheckLevel1);
                }
            }

            var adminId = _authManager.AdminId;
            var userId = _authManager.UserId;

            var contentInfo = new Content
            {
                SiteId = request.SiteId,
                ChannelId = request.ChannelId,
                AdminId = adminId,
                LastEditAdminId = adminId,
                UserId = userId,
                SourceId = request.SourceId,
                Checked = isChecked,
                CheckedLevel = checkedLevel
            };
            contentInfo.LoadDict(request.ToDictionary());

            contentInfo.Id = await _contentRepository.InsertAsync(site, channel, contentInfo);

            foreach (var plugin in _oldPluginManager.GetPlugins())
            {
                try
                {
                    plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(request.SiteId, request.ChannelId, contentInfo.Id, request.ToDictionary(), contentInfo));
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(IOldPlugin.ContentFormSubmit));
                }
            }

            if (contentInfo.Checked)
            {
                await _createManager.CreateContentAsync(request.SiteId, request.ChannelId, contentInfo.Id);
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, contentInfo.Id, "添加内容",
                $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

            return contentInfo;
        }

        [OpenApiOperation("修改内容API", "")]
        [HttpPut, Route(RouteContent)]
        public async Task<ActionResult<Content>> Update([FromBody]Content request)
        {
            bool isAuth;
            if (request.SourceId == SourceManager.User)
            {
                isAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Edit);
            }
            else
            {
                isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                         _authManager.IsUser &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Edit) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Edit);
            }
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channelInfo, request.Id);
            if (content == null) return NotFound();

            content.LoadDict(request.ToDictionary());

            content.SiteId = request.SiteId;
            content.ChannelId = request.ChannelId;
            content.LastEditAdminId = _authManager.AdminId;
            content.SourceId = request.SourceId;

            var postCheckedLevel = content.CheckedLevel;
            var isChecked = postCheckedLevel >= site.CheckContentLevel;
            var checkedLevel = postCheckedLevel;

            content.Checked = isChecked;
            content.CheckedLevel = checkedLevel;

            await _contentRepository.UpdateAsync(site, channelInfo, content);

            foreach (var plugin in _oldPluginManager.GetPlugins())
            {
                try
                {
                    plugin.OnContentFormSubmit(new ContentFormSubmitEventArgs(request.SiteId, request.ChannelId, content.Id, content.ToDictionary(), content));
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(IOldPlugin.ContentFormSubmit));
                }
            }

            if (content.Checked)
            {
                await _createManager.CreateContentAsync(request.SiteId, request.ChannelId, content.Id);
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, content.Id, "修改内容",
                $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, content.ChannelId)},内容标题:{content.Title}");

            return content;
        }

        [OpenApiOperation("删除内容API", "")]
        [HttpDelete, Route(RouteContent)]
        public async Task<ActionResult<Content>> Delete(int siteId, int channelId, int id)
        {
            var isUserAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.Delete);
            var isApiAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                            _authManager.IsUser &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.Delete) ||
                            _authManager.IsAdmin &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.Delete);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return NotFound();

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.Delete)) return Unauthorized();

            var content = await _contentRepository.GetAsync(site, channel, id);
            if (content == null) return NotFound();

            await _contentRepository.TrashContentAsync(site, channel, id, _authManager.AdminId);

            return content;
        }

        [OpenApiOperation("获取内容API", "")]
        [HttpGet, Route(RouteContent)]
        public async Task<ActionResult<Content>> Get(int siteId, int channelId, int id)
        {
            var isUserAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.View);
            var isApiAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                            _authManager.IsUser &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.View) ||
                            _authManager.IsAdmin &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.View);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(channelId);
            if (channelInfo == null) return NotFound();

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, AuthTypes.ContentPermissions.View)) return Unauthorized();

            var content = await _contentRepository.GetAsync(site, channelInfo, id);
            if (content == null) return NotFound();

            return content;
        }

        [OpenApiOperation("获取内容列表API", "")]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> GetContents([FromBody] QueryRequest request)
        {
            var channelId = request.ChannelId ?? request.SiteId;

            var isUserAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(request.SiteId, channelId, AuthTypes.ContentPermissions.View);
            var isApiAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                            _authManager.IsUser &&
                            await _authManager.HasContentPermissionsAsync(request.SiteId, channelId, AuthTypes.ContentPermissions.View) ||
                            _authManager.IsAdmin &&
                            await _authManager.HasContentPermissionsAsync(request.SiteId, channelId, AuthTypes.ContentPermissions.View);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var tableName = site.TableName;
            var query = await GetQueryAsync(request.SiteId, request.ChannelId, request);
            var totalCount = await _contentRepository.GetCountAsync(tableName, query);
            var summaries = await _contentRepository.GetSummariesAsync(tableName, query.ForPage(request.Page, request.PerPage));

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                contents.Add(content);
            }

            return new QueryResult
            {
                Contents = contents,
                TotalCount = totalCount
            };
        }

        [OpenApiOperation("审核内容API", "")]
        [HttpPost, Route(RouteActionsCheck)]
        public async Task<ActionResult<CheckResult>> CheckContents([FromBody] CheckRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var adminId = _authManager.AdminId;
            var contents = new List<Content>();
            foreach (var channelContentId in request.Contents)
            {
                var channel = await _channelRepository.GetAsync(channelContentId.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, channelContentId.Id);
                if (content == null) continue;

                content.Set(ColumnsManager.CheckAdminId, adminId);
                content.Set(ColumnsManager.CheckDate, DateTime.Now);
                content.Set(ColumnsManager.CheckReasons, request.Reasons);

                content.Checked = true;
                content.CheckedLevel = 0;

                await _contentRepository.UpdateAsync(site, channel, content);

                contents.Add(content);

                await _contentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = content.ChannelId,
                    ContentId = content.Id,
                    AdminId = adminId,
                    Checked = true,
                    CheckedLevel = 0,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量审核内容");

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
        //            isAuth = request.IsUserLoggin && await request.UserPermissions.HasChannelPermissionsAsync(siteId, siteId, AuthTypes.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated && await
        //                         DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     await request.UserPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                         AuthTypes.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     await request.AdminPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                         AuthTypes.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var site = await _siteRepository.GetAsync(siteId);
        //        if (site == null) return this.Error("无法确定内容对应的站点");

        //        if (!await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, siteId,
        //            AuthTypes.ChannelPermissions.ContentView)) return Unauthorized();

        //        var tableName = site.TableName;

        //        var parameters = new ApiContentsParameters(request);

        //        var (channelContentIds, count) = await _contentRepository.GetChannelContentIdListBySiteIdAsync(tableName, siteId, parameters);
        //        var value = new List<IDictionary<string, object>>();
        //        foreach (var channelContentId in channelContentIds)
        //        {
        //            var contentInfo = await _contentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
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
        //            isAuth = request.IsUserLoggin && await request.UserPermissions.HasChannelPermissionsAsync(siteId, channelId, AuthTypes.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated && await
        //                         DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     await request.UserPermissions.HasChannelPermissionsAsync(siteId, channelId,
        //                         AuthTypes.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
        //                         AuthTypes.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var site = await _siteRepository.GetAsync(siteId);
        //        if (site == null) return this.Error("无法确定内容对应的站点");

        //        var channelInfo = await _channelRepository.GetAsync(siteId, channelId);
        //        if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

        //        if (!await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
        //            AuthTypes.ChannelPermissions.ContentView)) return Unauthorized();

        //        var tableName = await _channelRepository.GetTableNameAsync(site, channelInfo);

        //        var top = request.GetQueryInt("top", 20);
        //        var skip = request.GetQueryInt("skip");
        //        var like = request.GetQueryString("like");
        //        var orderBy = request.GetQueryString("orderBy");

        //        var (list, count) = await _contentRepository.ApiGetContentIdListByChannelIdAsync(tableName, siteId, channelId, top, skip, like, orderBy, request.QueryString);
        //        var value = new List<IDictionary<string, object>>();
        //        foreach(var (contentChannelId, contentId) in list)
        //        {
        //            var contentInfo = await _contentRepository.GetAsync(site, contentChannelId, contentId);
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
        //        isAuth = req.IsUserLoggin && await req.UserPermissions.HasChannelPermissionsAsync(siteId, siteId, AuthTypes.ChannelPermissions.ContentView);
        //    }
        //    else
        //    {
        //        isAuth = req.IsApiAuthenticated && await
        //                     DataProvider.AccessTokenRepository.IsScopeAsync(req.ApiToken, Constants.ScopeContents) ||
        //                 req.IsUserLoggin &&
        //                 await req.UserPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                     AuthTypes.ChannelPermissions.ContentView) ||
        //                 req.IsAdminLoggin &&
        //                 await req.AdminPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                     AuthTypes.ChannelPermissions.ContentView);
        //    }
        //    if (!isAuth) return Request.Unauthorized<QueryResult>();

        //    var site = await _siteRepository.GetAsync(siteId);
        //    if (site == null) return Request.BadRequest<QueryResult>("无法确定内容对应的站点");

        //    if (!await req.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, siteId,
        //        AuthTypes.ChannelPermissions.ContentView)) return Request.Unauthorized<QueryResult>();

        //    var tableName = site.TableName;
        //    var query = GetQuery(siteId, null, request);
        //    var totalCount = await _contentRepository.GetTotalCountAsync(tableName, query);
        //    var channelContentIds = await _contentRepository.GetChannelContentIdListAsync(tableName, query);

        //    var contents = new List<Content>();
        //    foreach (var channelContentId in channelContentIds)
        //    {
        //        var content = await _contentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
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
