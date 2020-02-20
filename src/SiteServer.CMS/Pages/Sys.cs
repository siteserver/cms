using SiteServer.CMS.Api;

namespace SiteServer.CMS.Pages
{
    public static class Sys
    {
        public static string InnerApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');
    }
}
