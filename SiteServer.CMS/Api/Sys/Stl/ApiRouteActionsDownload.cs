using System.Collections.Specialized;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsDownload
    {
        public const string Route = "sys/stl/actions/download";

        public static string GetUrl(string apiUrl, int siteId, int channelId, int contentId)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()}
            });
        }

        public static string GetUrl(string apiUrl, int siteId, int channelId, int contentId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileUrl", TranslateUtils.EncryptStringBySecretKey(fileUrl)}
            });
        }

        public static string GetUrl(string apiUrl, int siteId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"fileUrl", TranslateUtils.EncryptStringBySecretKey(fileUrl)}
            });
        }

        public static string GetUrl(string apiUrl, string filePath)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"filePath", TranslateUtils.EncryptStringBySecretKey(filePath)}
            });
        }
    }
}