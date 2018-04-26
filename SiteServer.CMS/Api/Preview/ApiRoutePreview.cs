using SiteServer.Utils;

namespace SiteServer.CMS.Api.Preview
{
    public class ApiRoutePreview
    {
        public const string Route = "preview/{siteId}";
        public const string RouteChannel = "preview/{siteId}/{channelId}";
        public const string RouteContent = "preview/{siteId}/{channelId}/{contentId}";
        public const string RouteFile = "preview/{siteId}/file/{fileTemplateId}";
        public const string RouteSpecial = "preview/{siteId}/special/{specialId}";

        public static string GetSiteUrl(int siteId)
        {
            var apiUrl = ApiManager.GetInnerApiUrl(Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return apiUrl;
        }

        public static string GetChannelUrl(int siteId, int channelId)
        {
            var apiUrl = ApiManager.GetInnerApiUrl(RouteChannel);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            return apiUrl;
        }

        public static string GetContentUrl(int siteId, int channelId, int contentId)
        {
            var apiUrl = ApiManager.GetInnerApiUrl(RouteContent);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }

        public static string GetContentPreviewUrl(int siteId, int channelId, int contentId, int previewId)
        {
            if (contentId == 0)
            {
                contentId = previewId;
            }
            return $"{GetContentUrl(siteId, channelId, contentId)}?isPreview=true&previewId={previewId}";
        }

        public static string GetFileUrl(int siteId, int fileTemplateId)
        {
            var apiUrl = ApiManager.GetInnerApiUrl(RouteFile);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{fileTemplateId}", fileTemplateId.ToString());
            return apiUrl;
        }

        public static string GetSpecialUrl(int siteId, int specialId)
        {
            var apiUrl = ApiManager.GetInnerApiUrl(RouteSpecial);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{specialId}", specialId.ToString());
            return apiUrl;
        }

        public static string GetUrl(int siteId, int channelId, int contentId, int fileTemplateId, int specialId)
        {
            var apiUrl = GetSiteUrl(siteId);
            if (channelId > 0)
            {
                apiUrl = GetChannelUrl(siteId, channelId);
                if (contentId > 0)
                {
                    apiUrl = GetContentUrl(siteId, channelId, contentId);
                }
            }
            else if (fileTemplateId > 0)
            {
                apiUrl = GetFileUrl(siteId, fileTemplateId);
            }
            else if (specialId > 0)
            {
                apiUrl = GetSpecialUrl(siteId, specialId);
            }

            return apiUrl;
        }
    }
}