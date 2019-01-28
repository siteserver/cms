using SiteServer.CMS.Api;
using SiteServer.Utils;

namespace SiteServer.CMS.Pages
{
    public static class Env
    {
        public static string AdminDirectory => WebConfigUtils.AdminDirectory;

        public static string Path => PageUtils.ApplicationPath;

        public static string ApiPrefix => ApiManager.ApiPrefix;

        public static string AdminUrl => PageUtils.Combine(PageUtils.ApplicationPath, WebConfigUtils.AdminDirectory).TrimEnd('/');

        public static string ApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');
    }
}
