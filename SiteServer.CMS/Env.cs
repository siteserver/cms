using SiteServer.CMS.Core.RestRoutes;
using SiteServer.Utils;

namespace SiteServer.CMS
{
    public static class Env
    {
        public static string AdminUrl => PageUtils.Combine(PageUtils.ApplicationPath, WebConfigUtils.AdminDirectory).TrimEnd('/');

        public static string ApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');
    }
}
