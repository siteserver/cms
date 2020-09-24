using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerArticleController : ControllerBase
    {
        private const string RouteId = "common/editor/layerArticle/{id}";
        private const string RouteList = "common/editor/layerArticle/list";

        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialArticleRepository _materialArticleRepository;

        public LayerArticleController(IMaterialGroupRepository materialGroupRepository, IMaterialArticleRepository materialArticleRepository)
        {
            _materialGroupRepository = materialGroupRepository;
            _materialArticleRepository = materialArticleRepository;
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
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
