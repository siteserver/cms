using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Services;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ImageEditorController : ControllerBase
    {
        private const string Route = "cms/editor/imageEditor";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public ImageEditorController(
            IAuthManager authManager,
            IPathManager pathManager,
            ISiteRepository siteRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public string Base64String { get; set; }
        }
    }
}
