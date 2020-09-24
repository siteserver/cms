using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesSpecialEditorController : ControllerBase
    {
        private const string Route = "cms/templates/templatesSpecialEditor";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;

        public TemplatesSpecialEditorController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ISpecialRepository specialRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int SpecialId { get; set; }
        }

        public class GetResult
        {
            public Special Special { get; set; }
            public string SpecialUrl { get; set; }
            public string Html { get; set; }
        }
    }
}
