using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    [RoutePrefix("pages/cms/create/createFile")]
    public partial class PagesCreateFileController : ApiController
    {
        private const string Route = "";

        private readonly ICreateManager _createManager;

        public PagesCreateFileController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateFiles))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var templates =
                await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.FileTemplate);

            return new GetResult
            {
                Templates = templates
            };
        }
        

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Create([FromBody] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateFiles))
            {
                return Request.Unauthorized<BoolResult>();
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
