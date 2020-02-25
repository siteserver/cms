using System.Collections.Specialized;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Api.Stl
{
    public static class ApiRouteActionsDownload
    {
        public const string Route = "stl/actions/download";

        public static string GetUrl(string apiUrl, int siteId, int channelId, int contentId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileUrl", TranslateUtils.EncryptStringBySecretKey(fileUrl, WebConfigUtils.SecretKey)}
            });
        }

        public static string GetUrl(string apiUrl, int siteId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"fileUrl", TranslateUtils.EncryptStringBySecretKey(fileUrl, WebConfigUtils.SecretKey)}
            });
        }

        public static string GetUrl(string apiUrl, string filePath)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"filePath", TranslateUtils.EncryptStringBySecretKey(filePath, WebConfigUtils.SecretKey)}
            });
        }
    }
}