using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsLoadingChannels
    {
        public const string Route = "stl/actions/loading_channels";

        public static string GetUrl(string apiUrl)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            return apiUrl;
        }
    }
}