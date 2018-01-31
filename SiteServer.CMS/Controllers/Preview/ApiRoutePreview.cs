using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Preview
{
    public class ApiRoutePreview
    {
        public const string Route = "preview/{siteId}";
        public const string RouteChannel = "preview/{siteId}/{channelId}";
        public const string RouteContent = "preview/{siteId}/{channelId}/{contentId}";
        public const string RouteFile = "preview/{siteId}/file/{fileTemplateId}";

        public static string GetSiteUrl(int siteId)
        {
            return GetUrl(siteId, 0, 0, 0);
        }

        public static string GetChannelUrl(int siteId, int channelId)
        {
            return GetUrl(siteId, channelId, 0, 0);
        }

        public static string GetContentUrl(int siteId, int channelId, int contentId)
        {
            return GetUrl(siteId, channelId, contentId, 0);
        }

        public static string GetFileUrl(int siteId, int fileTemplateId)
        {
            return GetUrl(siteId, 0, 0, fileTemplateId);
        }

        private static string GetUrl(int siteId, int channelId, int contentId, int fileTemplateId)
        {
            var apiUrl = PageUtils.Combine(PageUtility.InnerApiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            if (channelId > 0)
            {
                apiUrl = PageUtils.Combine(apiUrl, channelId.ToString());
                if (contentId > 0)
                {
                    apiUrl = PageUtils.Combine(apiUrl, contentId.ToString());
                }
            }
            else if (fileTemplateId > 0)
            {
                apiUrl = PageUtils.Combine(apiUrl, fileTemplateId.ToString());
            }
            return apiUrl;
        }
    }
}