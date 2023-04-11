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
        private const string RouteContentUpdate = "contents/{siteId:int}/{channelId:int}/{id:int}/actions/update";
        private const string RouteContentDelete = "contents/{siteId:int}/{channelId:int}/{id:int}/actions/delete";

        public const string OpEquals = "=";
        public const string OpIn = "In";
        public const string OpNotIn = "NotIn";
        public const string OpLike = "Like";
        public const string OpNotLike = "NotLike";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IParseManager _parseManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public ContentsController(IAuthManager authManager, ICreateManager createManager, IParseManager parseManager, IDatabaseManager databaseManager, IPathManager pathManager, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentCheckRepository contentCheckRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _parseManager = parseManager;
            _databaseManager = databaseManager;
            _pathManager = pathManager;
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
                var channelIds = await _channelRepository.GetChannelIdsAsync(siteId, channelId.Value, ScopeType.All);

                query.WhereIn(nameof(Models.Content.ChannelId), channelIds);
            }

            if (request.Checked.HasValue)
            {
                query.Where(nameof(Models.Content.Checked), request.Checked);
            }
            if (request.Top.HasValue)
            {
                query.Where(nameof(Models.Content.Top), request.Top);
            }
            if (request.Recommend.HasValue)
            {
                query.Where(nameof(Models.Content.Recommend), request.Recommend);
            }
            if (request.Color.HasValue)
            {
                query.Where(nameof(Models.Content.Color), request.Color);
            }
            if (request.Hot.HasValue)
            {
                query.Where(nameof(Models.Content.Hot), request.Hot);
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
    }
}
