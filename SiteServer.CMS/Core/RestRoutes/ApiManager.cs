using SiteServer.CMS.Caches;
using SiteServer.Utils;

namespace SiteServer.CMS.Core.RestRoutes
{
    public static class ApiManager
    {
        public static bool IsSeparatedApi => ConfigManager.Instance.IsSeparatedApi;

        public static string ApiUrl => ConfigManager.Instance.ApiUrl;

        public static string RootUrl => PageUtils.ApplicationPath;

        public static string ApiPrefix => WebConfigUtils.ApiPrefix;

        public const string ApiVersion = "v1";

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
