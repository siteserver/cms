using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsLoadingChannels
    {
        public const string Route = "sys/stl/actions/loading_channels";

        public static string GetUrl(string apiUrl)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            return apiUrl;
        }
    }
}