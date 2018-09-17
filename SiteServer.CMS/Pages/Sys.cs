using SiteServer.CMS.Api;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.CMS.Pages
{
    public static class Sys
    {
        public static bool IsSeparatedApi => ConfigManager.SystemConfigInfo.IsSeparatedApi;

        public static string ApiUrl => ApiManager.ApiUrl.TrimEnd('/');

        public static string InnerApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');

        public static string RootUrl => PageUtils.ApplicationPath.TrimEnd('/');
    }
}
