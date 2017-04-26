using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class Star
    {
        public const string Route = "stl/star/{publishmentSystemId}/{channelId}/{contentId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId, int updaterId, int totalStar, int initStar, string theme, bool isStar)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"updaterId", updaterId.ToString() },
                {"totalStar", totalStar.ToString() },
                {"initStar", initStar.ToString() },
                {"theme", theme },
                {"isStar", isStar.ToString() },
                {"point", string.Empty }
            });
        }
    }
}