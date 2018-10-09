using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Core
{
    public static class SourceManager
	{
	    public const int User = -1;         //用户投稿
        public const int Preview = -99;     //预览
        public const int Default = 0;       //正常录入

        public static string GetSourceName(int sourceId)
        {
            if (sourceId == Default)
            {
                return "正常录入";
            }
            if (sourceId == User)
            {
                return "用户投稿";
            }
            if (sourceId == Preview)
            {
                return "预览插入";
            }
            if (sourceId <= 0) return string.Empty;

            var siteId = DataProvider.ChannelDao.GetSiteId(sourceId);
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            if (siteInfo == null) return "内容转移";

            var nodeNames = ChannelManager.GetChannelNameNavigation(siteId, sourceId);
            if (!string.IsNullOrEmpty(nodeNames))
            {
                return siteInfo.SiteName + "：" + nodeNames;
            }
            return siteInfo.SiteName;
        }
	}
}
