using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;
using ApiRouteVersion = SiteServer.CMS.Controllers.Sys.Packaging.ApiRouteVersion;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesVersionController : ApiController
    {
        public static string GetCacheKey(string packageId, bool allowNightlyBuild)
        {
            return $"SiteServer.API.Controllers.Sys.Packaging.PackagesVersionController.{packageId}-{allowNightlyBuild}";
        }

        [HttpGet, Route(ApiRouteVersion.RouteCms)]
        public IHttpActionResult VersionCms()
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

#if DEBUG
            return Ok();
#else
            var cacheKeyVersionAndNotes = GetCacheKey(PackageUtils.PackageIdSsCms, WebConfigUtils.AllowNightlyBuild);

            var versionAndNotes = CacheUtils.Get<VersionAndNotes>(cacheKeyVersionAndNotes);
            if (versionAndNotes == null)
            {
                string title;
                string version;
                string releaseNotes;
                DateTimeOffset? published;
                if (PackageUtils.FindLastPackage(PackageUtils.PackageIdSsCms, out title, out version, out published, out releaseNotes))
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
#endif
        }

        [HttpGet, Route(ApiRouteVersion.RoutePlugins)]
        public IHttpActionResult VersionPlugins()
        {
            var request = new Request();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

#if DEBUG
            return Ok();
#else

            var list = new List<VersionAndNotes>();

            var dict = PluginManager.GetPluginIdAndVersionDict();

            foreach (var pluginId in dict.Keys)
            {
                var installedVersion = dict[pluginId];

                var cacheKeyVersionAndNotes = GetCacheKey(pluginId, WebConfigUtils.AllowNightlyBuild);

                var versionAndNotes = CacheUtils.Get<VersionAndNotes>(cacheKeyVersionAndNotes);
                if (versionAndNotes == null)
                {
                    string title;
                    string version;
                    string releaseNotes;
                    DateTimeOffset? published;
                    if (PackageUtils.FindLastPackage(pluginId, out title, out version, out published, out releaseNotes))
                    {
                        versionAndNotes = new VersionAndNotes
                        {
                            Id = pluginId,
                            Title = title,
                            InstalledVersion = installedVersion,
                            Version = version,
                            Published = published,
                            ReleaseNotes = releaseNotes
                        };
                        CacheUtils.InsertHours(cacheKeyVersionAndNotes, versionAndNotes, 8);
                    }
                }
                if (versionAndNotes != null && versionAndNotes.Version != installedVersion)
                {
                    list.Add(versionAndNotes);
                }
            }

            return Ok(list);
#endif
        }
    }

    public class VersionAndNotes
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string InstalledVersion { get; set; }
        public string Version { get; set; }
        public DateTimeOffset? Published { get; set; }
        public string ReleaseNotes { get; set; }
    }
}
