using SiteServer.Abstractions;
using SiteServer.CMS.Context;

namespace SiteServer.CMS.Core
{
    public static class Extensions
    {
        public static string GetWebUrl(this Site site) => site.IsSeparatedWeb ? site.SeparatedWebUrl : PageUtils.ParseNavigationUrl($"~/{ site.SiteDir}");

        public static string GetAssetsUrl(this Site site) => site.IsSeparatedAssets ? site.SeparatedAssetsUrl : PageUtils.Combine(site.GetWebUrl(), site.AssetsDir);

        public static string GetApiUrl(this Config config) => config.IsSeparatedApi ? config.SeparatedApiUrl : PageUtils.ParseNavigationUrl("~/api");
    }
}
