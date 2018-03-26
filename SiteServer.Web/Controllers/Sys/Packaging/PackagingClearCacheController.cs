using System.Web.Http;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesClearCacheController : ApiController
    {
        [HttpPost, Route(ApiRouteClearCache.Route)]
        public IHttpActionResult Main()
        {
            var request = new AuthRequest();

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
