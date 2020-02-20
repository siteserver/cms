using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysPackagesClearCacheController : ApiController
    {
        private const string Route = "sys/packaging/clear/cache";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Main()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return Ok(new {});
        }
    }
}
