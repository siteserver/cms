using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    
    [RoutePrefix("pages/cms/create/createSpecial")]
    public partial class PagesCreateSpecialController : ApiController
    {
        private const string Route = "";

        private readonly ICreateManager _createManager;

        public PagesCreateSpecialController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateSpecials))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var specials =
                await DataProvider.SpecialRepository.GetSpecialListAsync(request.SiteId);

            return new GetResult
            {
                Specials = specials
            };
        }
        

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Create([FromBody] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateSpecials))
            {
                return Request.Unauthorized<BoolResult>();
            }

            foreach (var specialId in request.SpecialIds)
            {
                await _createManager.CreateSpecialAsync(request.SiteId, specialId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
