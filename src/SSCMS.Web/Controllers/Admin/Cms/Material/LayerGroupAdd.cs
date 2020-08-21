using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerGroupAddController : ControllerBase
    {
        private const string Route = "cms/material/layerGroupAdd";

        private readonly IAuthManager _authManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;

        public LayerGroupAddController(IAuthManager authManager, IMaterialGroupRepository materialGroupRepository)
        {
            _authManager = authManager;
            _materialGroupRepository = materialGroupRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public string GroupName { get; set; }
            public MaterialType LibraryType { get; set; }
        }

        public class CreateResult
        {
            public List<MaterialGroup> Groups { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
        }

        public class UpdateResult
        {
            public List<MaterialGroup> Groups { get; set; }
        }
    }
}
