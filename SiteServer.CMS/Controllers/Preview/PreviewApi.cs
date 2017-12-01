using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Preview
{
    public class PreviewApi
    {
        public const string Route = "preview/{publishmentSystemId}";
        public const string RouteChannel = "preview/{publishmentSystemId}/{channelId}";
        public const string RouteContent = "preview/{publishmentSystemId}/{channelId}/{contentId}";
        public const string RouteFile = "preview/{publishmentSystemId}/file/{fileTemplateId}";

        public static string GetPublishmentSystemUrl(int publishmentSystemId)
        {
            return GetUrl(publishmentSystemId, 0, 0, 0);
        }

        public static string GetChannelUrl(int publishmentSystemId, int channelId)
        {
            return GetUrl(publishmentSystemId, channelId, 0, 0);
        }

        public static string GetContentUrl(int publishmentSystemId, int channelId, int contentId)
        {
            return GetUrl(publishmentSystemId, channelId, channelId, 0);
        }

        public static string GetFileUrl(int publishmentSystemId, int fileTemplateId)
        {
            return GetUrl(publishmentSystemId, 0, 0, fileTemplateId);
        }

        //public static string GetPreviewUrl(int publishmentSystemId, int channelId, int contentId, int fileTemplateId, int pageIndex)
        //{
        //    var queryString = new NameValueCollection
        //    {
        //        {"s", publishmentSystemId.ToString()}
        //    };
        //    if (channelId > 0)
        //    {
        //        queryString.Add("n", channelId.ToString());
        //    }
        //    if (contentId > 0)
        //    {
        //        queryString.Add("c", contentId.ToString());
        //    }
        //    if (fileTemplateId > 0)
        //    {
        //        queryString.Add("f", fileTemplateId.ToString());
        //    }
        //    if (pageIndex > 0)
        //    {
        //        queryString.Add("p", pageIndex.ToString());
        //    }

        //    return PageUtils.GetSiteServerUrl("PagePreview", queryString);
        //}

        private static string GetUrl(int publishmentSystemId, int channelId, int contentId, int fileTemplateId)
        {
            var apiUrl = PageUtils.Combine(PageUtils.InnerApiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            if (channelId > 0 && contentId > 0)
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