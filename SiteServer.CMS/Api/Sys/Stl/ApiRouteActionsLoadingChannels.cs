using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsLoadingChannels
    {
        public const string Route = "sys/stl/actions/loading_channels";

        public static string GetUrl(string apiUrl)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            return apiUrl;
        }
    }
}