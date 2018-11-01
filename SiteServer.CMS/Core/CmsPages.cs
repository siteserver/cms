using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class CmsPages
    {
        public static string GetContentsUrl(int siteId, int channelId)
        {
            return PageUtils.GetCmsUrl("contents", siteId, new
            {
                channelId
            });
        }

        public static string GetCreateStatusUrl(int siteId)
        {
            return PageUtils.GetCmsUrl("createStatus", siteId);
        }
    }
}
