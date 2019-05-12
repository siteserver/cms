using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.CMS.Api
{
    public static class ApiManager
    {
        public static bool IsSeparatedApi => ConfigManager.Instance.IsSeparatedApi;

        public static string ApiUrl => ConfigManager.Instance.ApiUrl;

        public static string RootUrl => PageUtilsEx.ApplicationPath;

        private static string _innerApiUrl;

        public static string InnerApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_innerApiUrl))
                {
                    _innerApiUrl = PageUtilsEx.ParseNavigationUrl($"~/{WebConfigUtils.ApiPrefix}");
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
