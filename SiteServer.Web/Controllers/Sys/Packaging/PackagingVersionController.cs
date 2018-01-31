using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.Utils.Packaging;
using ApiRouteVersion = SiteServer.CMS.Controllers.Sys.Packaging.ApiRouteVersion;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesVersionController : ApiController
    {
        private const string CacheKeyVersionAndNotes = nameof(CacheKeyVersionAndNotes);

        public static string GetCacheKey(string packageId, bool allowNightlyBuild, string cackeKey)
        {
            return $"{packageId}-{allowNightlyBuild}-{cackeKey}";
        }

        [HttpGet, Route(ApiRouteVersion.Route)]
        public IHttpActionResult Main(string packageId)
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

#if DEBUG
            return Ok();
#endif

            var cacheKeyVersionAndNotes = GetCacheKey(packageId, WebConfigUtils.AllowNightlyBuild, CacheKeyVersionAndNotes);

            var versionAndNotes = CacheUtils.Get<VersionAndNotes>(cacheKeyVersionAndNotes);
            if (versionAndNotes == null)
            {
                string version;
                string releaseNotes;
                DateTimeOffset? published;
                if (PackageUtils.FindLastPackage(packageId, out version, out published, out releaseNotes))
                {
                    versionAndNotes = new VersionAndNotes
                    {
                        Version = version,
                        Published = published,
                        ReleaseNotes = releaseNotes
                    };
                    CacheUtils.InsertHours(cacheKeyVersionAndNotes, versionAndNotes, 8);
                }
            }

            return Ok(versionAndNotes);
        }
    }

    public class VersionAndNotes
    {
        public string Version { get; set; }
        public DateTimeOffset? Published { get; set; }
        public string ReleaseNotes { get; set; }
    }
}
