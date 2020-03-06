using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesEditor")]
    public partial class TemplatesEditorController : ControllerBase
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplatesEditorController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, ISiteRepository siteRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Default([FromQuery] TemplateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            Template template;
            if (request.TemplateId > 0)
            {
                template = await _templateRepository.GetAsync(request.TemplateId);
            }
            else
            {
                template = new Template
                {
                    TemplateType = request.TemplateType
                };
            }

            return new GetResult
            {
                Template = await GetTemplateResultAsync(template, site)
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Add([FromBody] Template request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveAsync(site, request);
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<GetResult>> Edit([FromBody] Template request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveAsync(site, request);
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] TemplateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await _createManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
