using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.Plugins.API
{
    public sealed class Paths
    {
        public static int GetSiteIdByFilePath(string path)
        {
            var publishmentSystemInfo = PathUtility.GetPublishmentSystemInfo(path);
            if (publishmentSystemInfo == null) return 0;

            return publishmentSystemInfo.PublishmentSystemId;
        }

        public static string GetSiteDirectoryPath(int siteId)
        {
            if (siteId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            if (publishmentSystemInfo == null) return null;

            return PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
        }
    }
}
