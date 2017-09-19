using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ContentApi : IContentApi
    {
        private ContentApi() { }

        public static ContentApi Instance { get; } = new ContentApi();

        public IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
        }

        public string GetContentValue(int publishmentSystemId, int channelId, int contentId, string attributeName)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, attributeName);
        }
    }
}
