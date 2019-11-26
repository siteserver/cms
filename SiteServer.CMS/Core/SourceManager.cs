using System.Threading.Tasks;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Core
{
    public static class SourceManager
	{
	    public const int User = -1;         //用户投稿
        public const int Preview = -99;     //预览
        public const int Default = 0;       //正常录入

        public static async Task<string> GetSourceNameAsync(int sourceId)
        {
            if (sourceId == Default)
            {
                return "后台录入";
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

            var sourceSiteId = await DataProvider.ChannelDao.GetSiteIdAsync(sourceId);
            var siteInfo = await DataProvider.SiteDao.GetAsync(sourceSiteId);
            if (siteInfo == null) return "内容转移";

            var nodeNames = await ChannelManager.GetChannelNameNavigationAsync(siteInfo.Id, sourceId);
            if (!string.IsNullOrEmpty(nodeNames))
            {
                return siteInfo.SiteName + "：" + nodeNames;
            }
            return siteInfo.SiteName;
        }
	}
}
