using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Plugins
{
    public class JsonApi
    {
        public const string Route = "plugins/{pluginId}";
        public const string RouteName = "plugins/{pluginId}/{name}";
        public const string RouteNameAndId = "plugins/{pluginId}/{name}/{id}";

        public static string GetUrl(string apiUrl, string pluginId, string name = "", int id = 0)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            if (!string.IsNullOrEmpty(name))
            {
                apiUrl = PageUtils.Combine(apiUrl, name);
                if (id > 0)
                {
                    apiUrl = PageUtils.Combine(apiUrl, id.ToString());
                }
            }
            return apiUrl;
        }
    }
}