using System.Threading.Tasks;
using SSCMS.Services;

namespace SSCMS.Core.Utils
{
    public static class SourceManager
	{
	    public const int User = -1;         //用户投稿
        public const int Preview = -99;     //预览
        public const int Default = 0;       //正常录入

        public static async Task<string> GetSourceNameAsync(IDatabaseManager databaseManager, int siteId, int referenceId, int sourceId)
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

            if (referenceId > 0)
            {
                var nodeNames = await databaseManager.ChannelRepository.GetChannelNameNavigationAsync(siteId, sourceId);
                if (!string.IsNullOrEmpty(nodeNames))
                {
                    return nodeNames;
                }
            }

            var sourceSiteId = await databaseManager.ChannelRepository.GetSiteIdAsync(sourceId);
            if (sourceSiteId == siteId)
            {
                var nodeNames = await databaseManager.ChannelRepository.GetChannelNameNavigationAsync(sourceSiteId, sourceId);
                if (!string.IsNullOrEmpty(nodeNames))
                {
                    return "从栏目转移：" + nodeNames;
                }
            }
            else
            {
                var siteInfo = await databaseManager.SiteRepository.GetAsync(sourceSiteId);
                if (siteInfo != null)
                {
                    return "从站点转移：" + siteInfo.SiteName;
                }
            }

            return string.Empty;
        }
    }
}
