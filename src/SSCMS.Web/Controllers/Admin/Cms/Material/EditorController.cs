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
    public partial class EditorController : ControllerBase
    {
        private const string Route = "cms/material/editor";
        private const string RouteActionsPreview = "cms/material/editor/actions/preview";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _openManager;
        private readonly IWxAccountRepository _openAccountRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;

        public EditorController(IAuthManager authManager, IWxManager openManager, IWxAccountRepository openAccountRepository, IMaterialMessageRepository materialMessageRepository)
        {
            _authManager = authManager;
            _openManager = openManager;
            _openAccountRepository = openAccountRepository;
            _materialMessageRepository = materialMessageRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int MessageId { get; set; }
        }

        public class GetResult
        {
            public List<MaterialMessageItem> Items { get; set; }
            public IEnumerable<Select<string>> CommentTypes { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public int GroupId { get; set; }

            public List<MaterialMessageItem> Items { get; set; }
        }

        public class CreateResult
        {
            public int MessageId { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int MessageId { get; set; }

            public int GroupId { get; set; }

            public List<MaterialMessageItem> Items { get; set; }
        }

        public class PreviewRequest : SiteRequest
        {
            public int MessageId { get; set; }

            public string WxNames { get; set; }
        }

        public class PreviewResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
