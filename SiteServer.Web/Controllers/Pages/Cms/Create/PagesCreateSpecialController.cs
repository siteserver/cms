using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    
    [RoutePrefix("pages/cms/create/createSpecial")]
    public partial class PagesCreateSpecialController : ApiController
    {
        private const string Route = "";

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
                await CreateManager.CreateSpecialAsync(request.SiteId, specialId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
