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
    public partial class EditorController : ControllerBase
    {
        private const string Route = "cms/material/editor";
        private const string RouteActionsPreview = "cms/material/editor/actions/preview";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;

        public EditorController(IAuthManager authManager, IWxManager wxManager, ISiteRepository siteRepository, IMaterialMessageRepository materialMessageRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _siteRepository = siteRepository;
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
            public string SiteType { get; set; }
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
            public int MaterialId { get; set; }

            public string WxNames { get; set; }
        }

        public class PreviewResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
