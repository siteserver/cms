using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PublishmentSystemApi : IPublishmentSystemApi
    {
        private PublishmentSystemApi() { }

        private static PublishmentSystemApi _instance;
        public static PublishmentSystemApi Instance => _instance ?? (_instance = new PublishmentSystemApi());

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

        public List<IPublishmentSystemInfo> GetPublishmentSystemInfoList(string adminName)
        {
            return PublishmentSystemManager.GetWritingPublishmentSystemInfoList(adminName);
        }
    }
}
