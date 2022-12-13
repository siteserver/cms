using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Core.Utils
{
    public static class SourceManager
    {
        public const int User = -1;         //用户投稿
        public const int Preview = -99;     //预览
        public const int Default = 0;       //正常录入

        public static async Task<string> GetSourceNameAsync(IDatabaseManager databaseManager, Content content)
        {
            if (content.SourceId == User)
            {
                return "用户投稿";
            }
            if (content.SourceId == Preview)
            {
                return "预览插入";
            }
            if (content.SourceId <= 0) return string.Empty;

            var sourceSiteId = await databaseManager.ChannelRepository.GetSiteIdAsync(content.SourceId);
            var sourceSite = await databaseManager.SiteRepository.GetAsync(sourceSiteId);
            if (sourceSite == null) return string.Empty;

            if (content.ReferenceId > 0)
            {
                if (sourceSiteId == content.SiteId)
                {
                    var nodeNames = await databaseManager.ChannelRepository.GetChannelNameNavigationAsync(sourceSiteId, content.SourceId);
                    if (!string.IsNullOrEmpty(nodeNames))
                    {
                        return "从栏目引用：" + nodeNames;
                    }
                }
                else
                {
                    return "从站点引用：" + sourceSite.SiteName;
                }
            }
            else
            {
                if (sourceSiteId == content.SiteId)
                {
                    var nodeNames = await databaseManager.ChannelRepository.GetChannelNameNavigationAsync(sourceSiteId, content.SourceId);
                    if (!string.IsNullOrEmpty(nodeNames))
                    {
                        return "从栏目转移：" + nodeNames;
                    }
                }
                else
                {
                    return "从站点转移：" + sourceSite.SiteName;
                }
            }

            return string.Empty;
        }
    }
}
