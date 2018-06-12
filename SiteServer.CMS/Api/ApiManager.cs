using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Api
{
    public static class ApiManager
    {
        private static string _innerApiUrl;

        public static string InnerApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_innerApiUrl))
                {
                    _innerApiUrl = PageUtils.ParseNavigationUrl("~/api");
                }
                return _innerApiUrl;
            }
        }

        public static string GetInnerApiUrl(string route)
        {
            return PageUtils.Combine(InnerApiUrl, route);
        }

        public static string ApiUrl => ConfigManager.SystemConfigInfo.ApiUrl;

        public static string GetApiUrl(string route)
        {
            return PageUtils.Combine(ApiUrl, route);
        }

        public static string SignalrHubsUrl => PageUtils.ParseNavigationUrl("~/signalr/hubs");
    }
}
