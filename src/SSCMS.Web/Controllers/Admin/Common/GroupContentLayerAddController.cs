using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class GroupContentLayerAddController : ControllerBase
    {
        private const string Route = "common/groupContentLayerAdd";

        private readonly IAuthManager _authManager;
        private readonly IContentGroupRepository _contentGroupRepository;

        public GroupContentLayerAddController(IAuthManager authManager, IContentGroupRepository contentGroupRepository)
        {
            _authManager = authManager;
            _contentGroupRepository = contentGroupRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class GetResult
        {
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class AddRequest : SiteRequest
        {
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class EditRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class ListResult
        {
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<ContentGroup> Groups { get; set; }
        }
    }
}
