using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PublishmentSystemApi : IPublishmentSystemApi
    {
        private PublishmentSystemApi() { }

        public static PublishmentSystemApi Instance { get; } = new PublishmentSystemApi();

        public int GetPublishmentSystemIdByFilePath(string path)
        {
            var publishmentSystemInfo = PathUtility.GetPublishmentSystemInfo(path);
            return publishmentSystemInfo?.PublishmentSystemId ?? 0;
        }

        public string GetPublishmentSystemPath(int publishmentSystemId)
        {
            if (publishmentSystemId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return publishmentSystemInfo == null ? null : PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
        }

        public List<int> GetPublishmentSystemIds()
        {
            return PublishmentSystemManager.GetPublishmentSystemIdList();
        }

        public IPublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId)
        {
            return PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
        }
    }
}
