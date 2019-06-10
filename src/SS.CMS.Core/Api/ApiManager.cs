using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Settings;
using SS.CMS.Utils;

namespace SS.CMS.Core.Api
{
    public static class ApiManager
    {
        public static bool IsSeparatedApi => ConfigManager.Instance.IsSeparatedApi;

        public static string ApiUrl => ConfigManager.Instance.ApiUrl;

        public static string RootUrl => AppContext.ApplicationPath;

        private static string _innerApiUrl;

        public static string InnerApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_innerApiUrl))
                {
                    _innerApiUrl = PageUtilsEx.ParseNavigationUrl($"~/{AppContext.ApiPrefix}");
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
