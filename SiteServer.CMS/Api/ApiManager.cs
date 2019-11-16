using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.CMS.Api
{
    public static class ApiManager
    {
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

        public static async Task<string> GetApiUrlAsync(string route = "")
        {
            var config = await ConfigManager.GetInstanceAsync();
            return PageUtils.Combine(config.ApiUrl, route);
        }

        public static string GetInnerApiUrl(string route)
        {
            return PageUtils.Combine(InnerApiUrl, route);
        }
    }
}
