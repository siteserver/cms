using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreateFileController : ControllerBase
    {
        private const string Route = "cms/create/createFile";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreateFileController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.CreateFiles))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var templates =
                await _templateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.FileTemplate);

            return new GetResult
            {
                Templates = templates
            };
        }
        

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.CreateFiles))
            {
                return Unauthorized();
            }

            foreach (var templateId in request.TemplateIds)
            {
                await _createManager.CreateFileAsync(request.SiteId, templateId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
