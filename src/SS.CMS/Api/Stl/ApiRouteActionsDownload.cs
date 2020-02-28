using System.Collections.Specialized;
using SS.CMS.Abstractions;
using SS.CMS.Core;

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
                {"fileUrl", GlobalSettings.SettingsManager.Encrypt(fileUrl)}
            });
        }

        public static string GetUrl(string apiUrl, int siteId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"fileUrl", GlobalSettings.SettingsManager.Encrypt(fileUrl)}
            });
        }

        public static string GetUrl(string apiUrl, string filePath)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"filePath", GlobalSettings.SettingsManager.Encrypt(filePath)}
            });
        }
    }
}