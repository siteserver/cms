using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin.Apis
{
    public class SiteApi : ISiteApi
    {
        private SiteApi() { }

        private static SiteApi _instance;
        public static SiteApi Instance => _instance ?? (_instance = new SiteApi());

        public int GetSiteIdByFilePath(string path)
        {
            var site = PathUtility.GetSiteAsync(path).GetAwaiter().GetResult();
            return site?.Id ?? 0;
        }

        public string GetSitePath(int siteId)
        {
            if (siteId <= 0) return null;

            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            return site == null ? null : PathUtility.GetSitePath(site);
        }

        public List<int> GetSiteIdList()
        {
            return SiteManager.GetSiteIdListAsync().GetAwaiter().GetResult();
        }

        public ISiteInfo GetSiteInfo(int siteId)
        {
            return SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
        }

        //public List<int> GetSiteIdListByAdminName(string adminName)
        //{
        //    var permissionManager = PermissionManager.GetInstance(adminName);
        //    return SiteManager.GetWritingSiteIdList(permissionManager);
        //}

        public string GetSitePath(int siteId, string virtualPath)
        {
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            return PathUtility.MapPath(site, virtualPath);
        }

        public string GetSiteUrl(int siteId)
        {
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            return PageUtility.GetSiteUrl(site, false);
        }

        public string GetSiteUrl(int siteId, string virtualPath)
        {
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            return PageUtility.ParseNavigationUrl(site, virtualPath, false);
        }

        public string GetSiteUrlByFilePath(string filePath)
        {
            var siteId = Instance.GetSiteIdByFilePath(filePath);
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            return PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, false).GetAwaiter().GetResult();
        }

        public void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls)
        {
            if (sourceSiteId == targetSiteId) return;

            var site = SiteManager.GetSiteAsync(sourceSiteId).GetAwaiter().GetResult();
            var targetSite = SiteManager.GetSiteAsync(targetSiteId).GetAwaiter().GetResult();
            if (site == null || targetSite == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    FileUtility.MoveFile(site, targetSite, relatedUrl);
                }
            }
        }

        public void AddWaterMark(int siteId, string filePath)
        {
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            FileUtility.AddWaterMark(site, filePath);
        }

        public string GetUploadFilePath(int siteId, string fileName)
        {
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, PathUtils.GetExtension(fileName));
            var localFileName = PathUtility.GetUploadFileName(site, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }
    }
}
