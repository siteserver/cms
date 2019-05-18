using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysPackagesClearCacheController : ApiController
    {
        [HttpPost, Route(ApiRouteClearCache.Route)]
        public IHttpActionResult Main()
        {
            var request = new Request(HttpContext.Current.Request);

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
