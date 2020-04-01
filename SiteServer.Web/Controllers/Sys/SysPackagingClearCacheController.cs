using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysPackagesClearCacheController : ApiController
    {
        private const string Route = "sys/packaging/clear/cache";

        [HttpPost, Route(Route)]
        public IHttpActionResult Main()
        {
            var request = new AuthenticatedRequest();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            CacheDbUtils.Clear();

            return Ok(new {});
        }
    }
}
