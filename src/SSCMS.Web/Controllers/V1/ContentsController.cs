using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    /// <summary>
    /// 内容操作API
    /// </summary>
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class ContentsController : ControllerBase
    {
        private const string Route = "contents";
        private const string RouteActionsCheck = "contents/actions/check";
        private const string RouteChannel = "contents/{siteId:int}/{channelId:int}";
        private const string RouteContent = "contents/{siteId:int}/{channelId:int}/{id:int}";

        public const string OpEquals = "=";
        public const string OpIn = "In";
        public const string OpNotIn = "NotIn";
        public const string OpLike = "Like";
        public const string OpNotLike = "NotLike";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public ContentsController(IAuthManager authManager, ICreateManager createManager, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentCheckRepository contentCheckRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _accessTokenRepository = accessTokenRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentCheckRepository = contentCheckRepository;
        }

        public class ClauseWhere
        {
            public string Column { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }

        public class ClauseOrder
        {
            public string Column { get; set; }
            public bool Desc { get; set; }
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public int? ChannelId { get; set; }
            public bool? Checked { get; set; }
            public bool? Top { get; set; }
            public bool? Recommend { get; set; }
            public bool? Color { get; set; }
            public bool? Hot { get; set; }
            public List<string> GroupNames { get; set; }
            public List<string> TagNames { get; set; }
            public List<ClauseWhere> Wheres { get; set; }
            public List<ClauseOrder> Orders { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public int TotalCount { get; set; }
            public IEnumerable<Content> Contents { get; set; }
        }

        public class CheckRequest
        {
            public int SiteId { get; set; }
            public List<ContentSummary> Contents { get; set; }
            public string Reasons { get; set; }
        }

        public class CheckResult
        {
            public List<Content> Contents { get; set; }
        }

        private async Task<Query> GetQueryAsync(int siteId, int? channelId, QueryRequest request)
        {
            var query = Q.Where(nameof(Models.Content.SiteId), siteId).Where(nameof(Models.Content.ChannelId), ">", 0);

            if (channelId.HasValue)
            {
                //query.Where(nameof(Abstractions.Content.ChannelId), channelId.Value);
                var channelIds = await _channelRepository.GetChannelIdsAsync(siteId, channelId.Value, ScopeType.All);

                query.WhereIn(nameof(Models.Content.ChannelId), channelIds);
            }

            if (request.Checked.HasValue)
            {
                query.Where(nameof(Models.Content.Checked), request.Checked.Value.ToString());
            }
            if (request.Top.HasValue)
            {
                query.Where(nameof(Models.Content.Top), request.Top.Value.ToString());
            }
            if (request.Recommend.HasValue)
            {
                query.Where(nameof(Models.Content.Recommend), request.Recommend.Value.ToString());
            }
            if (request.Color.HasValue)
            {
                query.Where(nameof(Models.Content.Color), request.Color.Value.ToString());
            }
            if (request.Hot.HasValue)
            {
                query.Where(nameof(Models.Content.Hot), request.Hot.Value.ToString());
            }

            if (request.GroupNames != null)
            {
                query.Where(q =>
                {
                    foreach (var groupName in request.GroupNames)
                    {
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            q
                                .OrWhere(nameof(Models.Content.GroupNames), groupName)
                                .OrWhereLike(nameof(Models.Content.GroupNames), $"{groupName},%")
                                .OrWhereLike(nameof(Models.Content.GroupNames), $"%,{groupName},%")
                                .OrWhereLike(nameof(Models.Content.GroupNames), $"%,{groupName}");
                        }
                    }
                    return q;
                });
            }

            if (request.TagNames != null)
            {
                query.Where(q =>
                {
                    foreach (var tagName in request.TagNames)
                    {
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            q
                                .OrWhere(nameof(Models.Content.TagNames), tagName)
                                .OrWhereLike(nameof(Models.Content.TagNames), $"{tagName},%")
                                .OrWhereLike(nameof(Models.Content.TagNames), $"%,{tagName},%")
                                .OrWhereLike(nameof(Models.Content.TagNames), $"%,{tagName}");
                        }
                    }
                    return q;
                });
            }

            if (request.Wheres != null)
            {
                foreach (var where in request.Wheres)
                {
                    if (string.IsNullOrEmpty(where.Operator)) where.Operator = OpEquals;
                    if (StringUtils.EqualsIgnoreCase(where.Operator, OpIn))
                    {
                        query.WhereIn(where.Column, ListUtils.GetStringList(where.Value));
                    }
                    else if (StringUtils.EqualsIgnoreCase(where.Operator, OpNotIn))
                    {
                        query.WhereNotIn(where.Column, ListUtils.GetStringList(where.Value));
                    }
                    else if (StringUtils.EqualsIgnoreCase(where.Operator, OpLike))
                    {
                        query.WhereLike(where.Column, where.Value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(where.Operator, OpNotLike))
                    {
                        query.WhereNotLike(where.Column, where.Value);
                    }
                    else
                    {
                        query.Where(where.Column, where.Operator, where.Value);
                    }
                }
            }

            if (request.Orders != null)
            {
                foreach (var order in request.Orders)
                {
                    if (order.Desc)
                    {
                        query.OrderByDesc(order.Column);
                    }
                    else
                    {
                        query.OrderBy(order.Column);
                    }
                }
            }
            else
            {
                query.OrderByDesc(nameof(Models.Content.Top), 
                    nameof(Models.Content.ChannelId),
                    nameof(Models.Content.Taxis),
                    nameof(Models.Content.Id));
            }

            return query;
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
        //            isAuth = request.IsUserLoggin && await request.UserPermissions.HasChannelPermissionsAsync(siteId, siteId, AuthMenuUtils.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated && await
        //                         DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     await request.UserPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                         AuthMenuUtils.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     await request.AdminPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                         AuthMenuUtils.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var site = await _siteRepository.GetAsync(siteId);
        //        if (site == null) return this.Error("无法确定内容对应的站点");

        //        if (!await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, siteId,
        //            AuthMenuUtils.ChannelPermissions.ContentView)) return Unauthorized();

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
        //            isAuth = request.IsUserLoggin && await request.UserPermissions.HasChannelPermissionsAsync(siteId, channelId, AuthMenuUtils.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated && await
        //                         DataProvider.AccessTokenRepository.IsScopeAsync(request.ApiToken, Constants.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     await request.UserPermissions.HasChannelPermissionsAsync(siteId, channelId,
        //                         AuthMenuUtils.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     await request.AdminPermissions.HasChannelPermissionsAsync(siteId, channelId,
        //                         AuthMenuUtils.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var site = await _siteRepository.GetAsync(siteId);
        //        if (site == null) return this.Error("无法确定内容对应的站点");

        //        var channelInfo = await _channelRepository.GetAsync(siteId, channelId);
        //        if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

        //        if (!await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
        //            AuthMenuUtils.ChannelPermissions.ContentView)) return Unauthorized();

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
        //        isAuth = req.IsUserLoggin && await req.UserPermissions.HasChannelPermissionsAsync(siteId, siteId, AuthMenuUtils.ChannelPermissions.ContentView);
        //    }
        //    else
        //    {
        //        isAuth = req.IsApiAuthenticated && await
        //                     DataProvider.AccessTokenRepository.IsScopeAsync(req.ApiToken, Constants.ScopeContents) ||
        //                 req.IsUserLoggin &&
        //                 await req.UserPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                     AuthMenuUtils.ChannelPermissions.ContentView) ||
        //                 req.IsAdminLoggin &&
        //                 await req.AdminPermissions.HasChannelPermissionsAsync(siteId, siteId,
        //                     AuthMenuUtils.ChannelPermissions.ContentView);
        //    }
        //    if (!isAuth) return Request.Unauthorized<QueryResult>();

        //    var site = await _siteRepository.GetAsync(siteId);
        //    if (site == null) return Request.BadRequest<QueryResult>("无法确定内容对应的站点");

        //    if (!await req.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, siteId,
        //        AuthMenuUtils.ChannelPermissions.ContentView)) return Request.Unauthorized<QueryResult>();

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
