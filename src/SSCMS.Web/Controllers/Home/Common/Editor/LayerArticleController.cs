using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class LayerArticleController : ControllerBase
    {
        private const string RouteId = "common/editor/layerArticle/{id}";
        private const string RouteList = "common/editor/layerArticle/list";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialArticleRepository _materialArticleRepository;

        public LayerArticleController(IAuthManager authManager, IConfigRepository configRepository, IMaterialGroupRepository materialGroupRepository, IMaterialArticleRepository materialArticleRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialArticleRepository = materialArticleRepository;
        }

        public class QueryRequest : SiteRequest
        {
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public bool IsSiteOnly { get; set; }
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialArticle> Items { get; set; }
        }

        public class GroupRequest
        {
            public int SiteId { get; set; }
            public string Name { get; set; }
        }
    }
}
