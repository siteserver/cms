using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class SiteApi : ISiteApi
    {
        private SiteApi() { }

        private static SiteApi _instance;
        public static SiteApi Instance => _instance ?? (_instance = new SiteApi());

        public int GetSiteIdByFilePath(string path)
        {
            var siteInfo = PathUtility.GetSiteInfo(path);
            return siteInfo?.Id ?? 0;
        }

        public string GetSitePath(int siteId)
        {
            if (siteId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return siteInfo == null ? null : PathUtility.GetSitePath(siteInfo);
        }

        public List<int> GetSiteIdList()
        {
            return SiteManager.GetSiteIdList();
        }

        public ISiteInfo GetSiteInfo(int siteId)
        {
            return SiteManager.GetSiteInfo(siteId);
        }

        public List<ISiteInfo> GetSiteInfoList(string adminName)
        {
            return SiteManager.GetWritingSiteInfoList(adminName);
        }
    }
}
