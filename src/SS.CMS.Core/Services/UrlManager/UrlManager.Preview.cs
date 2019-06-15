using SS.CMS.Abstractions.Services;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public string PreviewRoute => "preview/{siteId}";
        public string PreviewRouteChannel => "preview/{siteId}/{channelId}";
        public string PreviewRouteContent => "preview/{siteId}/{channelId}/{contentId}";
        public string PreviewRouteFile => "preview/{siteId}/file/{fileTemplateId}";
        public string PreviewRouteSpecial => "preview/{siteId}/special/{specialId}";

        public string GetPreviewSiteUrl(int siteId)
        {
            var apiUrl = GetInnerApiUrl(PreviewRoute);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return apiUrl;
        }

        public string GetPreviewChannelUrl(int siteId, int channelId)
        {
            var apiUrl = GetInnerApiUrl(PreviewRouteChannel);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            return apiUrl;
        }

        public string GetPreviewContentUrl(int siteId, int channelId, int contentId)
        {
            var apiUrl = GetInnerApiUrl(PreviewRouteContent);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }

        public string GetPreviewContentPreviewUrl(int siteId, int channelId, int contentId, int previewId)
        {
            if (contentId == 0)
            {
                contentId = previewId;
            }
            return $"{GetPreviewContentUrl(siteId, channelId, contentId)}?isPreview=true&previewId={previewId}";
        }

        public string GetPreviewFileUrl(int siteId, int fileTemplateId)
        {
            var apiUrl = GetInnerApiUrl(PreviewRouteFile);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{fileTemplateId}", fileTemplateId.ToString());
            return apiUrl;
        }

        public string GetPreviewSpecialUrl(int siteId, int specialId)
        {
            var apiUrl = GetInnerApiUrl(PreviewRouteSpecial);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{specialId}", specialId.ToString());
            return apiUrl;
        }

        public string GetPreviewUrl(int siteId, int channelId, int contentId, int fileTemplateId, int specialId)
        {
            var apiUrl = GetPreviewSiteUrl(siteId);
            if (channelId > 0)
            {
                apiUrl = GetPreviewChannelUrl(siteId, channelId);
                if (contentId > 0)
                {
                    apiUrl = GetPreviewContentUrl(siteId, channelId, contentId);
                }
            }
            else if (fileTemplateId > 0)
            {
                apiUrl = GetPreviewFileUrl(siteId, fileTemplateId);
            }
            else if (specialId > 0)
            {
                apiUrl = GetPreviewSpecialUrl(siteId, specialId);
            }

            return apiUrl;
        }
    }
}