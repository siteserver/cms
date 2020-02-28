using System.Threading.Tasks;
using SS.CMS.Core;
using SS.CMS.Extensions;

namespace SS.CMS.Api
{
    public static class ApiManager
    {
        //public static string RootUrl => PageUtils.ApplicationPath;

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

        //public static async Task<string> GetApiUrlAsync(string route = "")
        //{
        //    var config = await GlobalSettings.ConfigRepository.GetAsync();
        //    return PageUtils.Combine(config.GetApiUrl(), route);
        //}

        public static string GetInnerApiUrl(string route)
        {
            return PageUtils.Combine(InnerApiUrl, route);
        }
    }
}
