using System.Collections.Generic;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerArticlesController : ControllerBase
    {
        private const string Route = "cms/material/layerArticles";

        private readonly IAuthManager _authManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialArticleRepository _materialArticleRepository;

        public LayerArticlesController(IAuthManager authManager, IMaterialGroupRepository materialGroupRepository, IMaterialArticleRepository materialArticleRepository)
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
