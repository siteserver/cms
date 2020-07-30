using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class MessageController : ControllerBase
    {
        private const string Route = "cms/material/message";
        private const string RouteActionsDeleteGroup = "cms/material/message/actions/deleteGroup";
        private const string RouteActionsPull = "cms/material/message/actions/pull";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _openManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;
        private readonly IWxAccountRepository _openAccountRepository;

        public MessageController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IWxManager openManager, ISiteRepository siteRepository, IMaterialGroupRepository materialGroupRepository, IMaterialMessageRepository materialMessageRepository, IWxAccountRepository openAccountRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _openManager = openManager;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialMessageRepository = materialMessageRepository;
            _openAccountRepository = openAccountRepository;
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
            public IEnumerable<MaterialMessage> Messages { get; set; }
            public bool IsOpen { get; set; }
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

        public class PullRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class PullResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
