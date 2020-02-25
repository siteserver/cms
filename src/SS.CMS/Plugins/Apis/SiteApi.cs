using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS;

using SS.CMS.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Plugin.Apis
{
    public class SiteApi
    {
        private SiteApi() { }

        private static SiteApi _instance;
        public static SiteApi Instance => _instance ??= new SiteApi();

        public async Task<int> GetSiteIdByFilePathAsync(string path)
        {
            var site = await PathUtility.GetSiteAsync(path);
            return site?.Id ?? 0;
        }

        public async Task<string> GetSitePathAsync(int siteId)
        {
            if (siteId <= 0) return null;

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            return site == null ? null : await PathUtility.GetSitePathAsync(site);
        }

        public async Task<IEnumerable<int>> GetSiteIdListAsync()
        {
            return await DataProvider.SiteRepository.GetSiteIdListAsync();
        }

        public async Task<Site> GetSiteAsync(int siteId)
        {
            return await DataProvider.SiteRepository.GetAsync(siteId);
        }

        //public List<int> GetSiteIdListByAdminName(string adminName)
        //{
        //    var permissionManager = PermissionManager.GetInstance(adminName);
        //    return DataProvider.SiteRepository.GetWritingSiteIdList(permissionManager);
        //}

        public async Task<string> GetSitePathAsync(int siteId, string virtualPath)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            return await PathUtility.MapPathAsync(site, virtualPath);
        }

        public async Task<string> GetSiteUrlAsync(int siteId)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            return await PageUtility.GetSiteUrlAsync(site, false);
        }

        public async Task<string> GetSiteUrlAsync(int siteId, string virtualPath)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            return await PageUtility.ParseNavigationUrlAsync(site, virtualPath, false);
        }

        public async Task<string> GetSiteUrlByFilePathAsync(string filePath)
        {
            var siteId = await Instance.GetSiteIdByFilePathAsync(filePath);
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            return await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, false);
        }

        public async Task MoveFilesAsync(int sourceSiteId, int targetSiteId, List<string> relatedUrls)
        {
            if (sourceSiteId == targetSiteId) return;

            var site = await DataProvider.SiteRepository.GetAsync(sourceSiteId);
            var targetSite = await DataProvider.SiteRepository.GetAsync(targetSiteId);
            if (site == null || targetSite == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    await FileUtility.MoveFileAsync(site, targetSite, relatedUrl);
                }
            }
        }

        public async Task AddWaterMarkAsync(int siteId, string filePath)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            await FileUtility.AddWaterMarkAsync(site, filePath);
        }

        public async Task<string> GetUploadFilePathAsync(int siteId, string fileName)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var localDirectoryPath = await PathUtility.GetUploadDirectoryPathAsync(site, PathUtils.GetExtension(fileName));
            var localFileName = PathUtility.GetUploadFileName(site, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }
    }
}
