using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysPackagesClearCacheController : ControllerBase
    {
        [HttpPost, Route(ApiRouteClearCache.Route)]
        public IHttpActionResult Main()
        {
            var request = GetRequest();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            CacheDbUtils.Clear();

            return Ok(new { });
        }
    }
}
