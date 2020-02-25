using System.Threading.Tasks;
using SS.CMS.Framework;

namespace SS.CMS.Core
{
    public static class SourceManager
	{
	    public const int User = -1;         //用户投稿
        public const int Preview = -99;     //预览
        public const int Default = 0;       //正常录入

        public static async Task<string> GetSourceNameAsync(int siteId, int sourceId)
        {
            if (sourceId == User)
            {
                return "用户投稿";
            }
            if (sourceId == Preview)
            {
                return "预览插入";
            }
            if (sourceId <= 0) return string.Empty;

            var sourceSiteId = await DataProvider.ChannelRepository.GetSiteIdAsync(sourceId);
            if (sourceSiteId == siteId)
            {
                var nodeNames = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(sourceSiteId, sourceId);
                if (!string.IsNullOrEmpty(nodeNames))
                {
                    return "从栏目转移：" + nodeNames;
                }
            }
            else
            {
                var siteInfo = await DataProvider.SiteRepository.GetAsync(sourceSiteId);
                if (siteInfo != null)
                {
                    return "从站点转移：" + siteInfo.SiteName;
                }
            }

            return "后台录入";
        }
	}
}
