namespace SS.CMS.Core.Common
{
    public static class CmsPages
    {
        public static string GetContentsUrl(int siteId, int channelId)
        {
            return PageUtilsEx.GetCmsUrl("contents", siteId, new
            {
                channelId
            });
        }

        public static string GetCreateStatusUrl(int siteId)
        {
            return PageUtilsEx.GetCmsUrl("createStatus", siteId);
        }
    }
}
