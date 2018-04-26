using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Errors
{
    public class ApiRouteError
    {
        public const string Route = "sys/errors/{id}";

        public static string GetUrl(string apiUrl, int id)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{id}", id.ToString());
            return apiUrl;
        }
    }
}