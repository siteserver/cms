using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysPackagesClearCacheController : ApiController
    {
        private const string Route = "sys/packaging/clear/cache";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Main()
        {
            var request = await AuthenticatedRequest.GetRequestAsync();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            await DataProvider.DbCacheDao.ClearAsync();

            return Ok(new {});
        }
    }
}
