using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.CMS.Api
{
    public static class ApiManager
    {
        public static bool IsSeparatedApi => ConfigManager.SystemConfigInfo.IsSeparatedApi;

        public static string ApiUrl => ConfigManager.SystemConfigInfo.ApiUrl;

        public static string RootUrl => PageUtils.ApplicationPath;

        private static string _innerApiUrl;

        public static string InnerApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_innerApiUrl))
                {
                    _innerApiUrl = PageUtils.ParseNavigationUrl($"~/{WebConfigUtils.ApiPrefix}");
                }
                return _innerApiUrl;
            }
        }

        public static string GetApiUrl(string route)
        {
            return PageUtils.Combine(ApiUrl, route);
        }

        public static string GetInnerApiUrl(string route)
        {
            return PageUtils.Combine(InnerApiUrl, route);
        }
    }
}
