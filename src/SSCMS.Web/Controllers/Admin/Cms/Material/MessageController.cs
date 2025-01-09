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
    public partial class MessageController : ControllerBase
    {
        private const string Route = "cms/material/message";
        private const string RouteUpdate = "cms/material/message/actions/update";
        private const string RouteDelete = "cms/material/message/actions/delete";
        private const string RouteDeleteGroup = "cms/material/message/actions/deleteGroup";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _wxManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;

        public MessageController(
            IAuthManager authManager,
            IPathManager pathManager,
            IWxManager wxManager,
            IConfigRepository configRepository,
            ISiteRepository siteRepository,
            IMaterialGroupRepository materialGroupRepository,
            IMaterialMessageRepository materialMessageRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _wxManager = wxManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialMessageRepository = materialMessageRepository;
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
            public bool IsWxEnabled { get; set; }
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialMessage> Messages { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int Id { get; set; }
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
