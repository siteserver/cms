using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerArticleSelectController : ControllerBase
    {
        private const string Route = "common/material/layerArticleSelect";

        private readonly IAuthManager _authManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialArticleRepository _materialArticleRepository;

        public LayerArticleSelectController(IAuthManager authManager, IMaterialGroupRepository materialGroupRepository, IMaterialArticleRepository materialArticleRepository)
        {
            _authManager = authManager;
            _materialGroupRepository = materialGroupRepository;
            _materialArticleRepository = materialArticleRepository;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
            public string ArticleIds { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialArticle> Items { get; set; }
        }
    }
}
