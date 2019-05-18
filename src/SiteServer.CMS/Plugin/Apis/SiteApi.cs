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

        //public List<int> GetSiteIdListByAdminName(string adminName)
        //{
        //    var permissionManager = PermissionManager.GetInstance(adminName);
        //    return SiteManager.GetWritingSiteIdList(permissionManager);
        //}

        public string GetSitePath(int siteId, string virtualPath)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PathUtility.MapPath(siteInfo, virtualPath);
        }

        public string GetSiteUrl(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetSiteUrl(siteInfo, false);
        }

        public string GetSiteUrl(int siteId, string virtualPath)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.ParseNavigationUrl(siteInfo, virtualPath, false);
        }

        public string GetSiteUrlByFilePath(string filePath)
        {
            var siteId = Instance.GetSiteIdByFilePath(filePath);
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetSiteUrlByPhysicalPath(siteInfo, filePath, false);
        }

        public void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls)
        {
            if (sourceSiteId == targetSiteId) return;

            var siteInfo = SiteManager.GetSiteInfo(sourceSiteId);
            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
            if (siteInfo == null || targetSiteInfo == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    FileUtility.MoveFile(siteInfo, targetSiteInfo, relatedUrl);
                }
            }
        }

        public void AddWaterMark(int siteId, string filePath)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            FileUtility.AddWaterMark(siteInfo, filePath);
        }

        public string GetUploadFilePath(int siteId, string fileName)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, PathUtils.GetExtension(fileName));
            var localFileName = PathUtility.GetUploadFileName(siteInfo, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }
    }
}
