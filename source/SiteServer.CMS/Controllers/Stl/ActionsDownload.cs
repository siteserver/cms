using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsDownload
    {
        public const string Route = "stl/actions/download";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()}
            });
        }

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileUrl", TranslateUtils.EncryptStringBySecretKey(fileUrl)}
            });
        }

        public static string GetUrl(string apiUrl, int publishmentSystemId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
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