using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templatesEditor")]
    public partial class PagesTemplateEditorController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        private readonly ICreateManager _createManager;

        public PagesTemplateEditorController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> Default([FromUri] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            Template template;
            if (request.TemplateId > 0)
            {
                template = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);
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
        public async Task<GetResult> Add([FromBody] Template request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            return await SaveAsync(site, request, auth);
        }

        [HttpPut, Route(Route)]
        public async Task<GetResult> Edit([FromBody] Template request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            return await SaveAsync(site, request, auth);
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<BoolResult> Create([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            await _createManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
