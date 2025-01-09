using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ComponentController : ControllerBase
    {
        private const string Route = "cms/material/component";
        private const string RouteUpdate = "cms/material/component/actions/update";
        private const string RouteDelete = "cms/material/component/actions/delete";
        private const string RouteDeleteGroup = "cms/material/component/actions/deleteGroup";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialComponentRepository _materialComponentRepository;

        public ComponentController(
            IAuthManager authManager,
            IConfigRepository configRepository,
            ISiteRepository siteRepository,
            IMaterialGroupRepository materialGroupRepository,
            IMaterialComponentRepository materialComponentRepository
        )
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialComponentRepository = materialComponentRepository;
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
            public bool IsSiteOnly { get; set; }
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialComponent> Items { get; set; }
            public string SiteType { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int GroupId { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int Id { get; set; }
        }

        public class DeleteGroupRequest : SiteRequest
        {
            public int Id { get; set; }
        }
    }
}
