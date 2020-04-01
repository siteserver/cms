using SiteServer.CMS.Api;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.CMS.Pages
{
    public static class Sys
    {
        public static string InnerApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');
    }
}
