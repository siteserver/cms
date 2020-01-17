using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/templateEditor")]
    public partial class PagesTemplateEditorController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Default([FromUri] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.Templates);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            Template template;
            if (request.TemplateId > 0)
            {
                template = await TemplateManager.GetTemplateAsync(site.Id, request.TemplateId);
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
                Template = GetTemplateResult(template, site)
            };
        }

        [HttpPost, Route(Route)]
        public async Task<GetResult> Add([FromBody] Template request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.Templates);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            return await SaveAsync(site, request, auth);
        }

        [HttpPut, Route(Route)]
        public async Task<GetResult> Edit([FromBody] Template request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.Templates);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            return await SaveAsync(site, request, auth);
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<DefaultResult> Create([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.Templates);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<DefaultResult>();

            await CreateManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
