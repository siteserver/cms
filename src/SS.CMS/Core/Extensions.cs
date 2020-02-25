using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Core
{
    public static class Extensions
    {
        public static async Task<string> GetWebUrlAsync(this Site site) => site.IsSeparatedWeb ? site.SeparatedWebUrl : await PageUtility.GetLocalSiteUrlAsync(site);

        public static async Task<string> GetAssetsUrlAsync(this Site site) => site.IsSeparatedAssets ? site.SeparatedAssetsUrl : PageUtils.Combine(await site.GetWebUrlAsync(), site.AssetsDir);

        public static string GetApiUrl(this Config config) => config.IsSeparatedApi ? config.SeparatedApiUrl : PageUtils.ParseNavigationUrl("~/api");
    }
}
