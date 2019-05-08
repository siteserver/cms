using System.Collections.Generic;
using System.Net.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes;
using SiteServer.CMS.Fx;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Apis
{
    public class UtilsApi : IUtilsApi
    {
        private UtilsApi() { }

        private static UtilsApi _instance;
        public static UtilsApi Instance => _instance ?? (_instance = new UtilsApi());

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString);
        }

        public string FilterXss(string html)
        {
            return AttackUtils.FilterXss(html);
        }

        public string FilterSql(string sql)
        {
            return AttackUtils.FilterSql(sql);
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

        public string GetUploadFileUrl(int siteId, string fileName)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, PathUtils.GetExtension(fileName));
            var localFileName = PathUtility.GetUploadFileName(siteInfo, fileName);
            var path = PathUtils.Combine(localDirectoryPath, localFileName);
            return PageUtility.GetSiteUrlByPhysicalPath(siteInfo, path, false);
        }

        public string GetTemporaryFilesPath(string relatedPath)
        {
            return PathUtils.GetTemporaryFilesPath(relatedPath);
        }

        public string GetRootUrl(string relatedUrl = "")
        {
            return FxUtils.GetRootUrl(relatedUrl);
        }

        public string GetAdminUrl(string relatedUrl = "")
        {
            return FxUtils.GetAdminUrl(relatedUrl);
        }

        public string GetHomeUrl(string relatedUrl = "")
        {
            return FxUtils.GetHomeUrl(relatedUrl);
        }

        public string GetApiUrl(string relatedUrl = "")
        {
            return ApiManager.GetApiUrl(relatedUrl);
        }

        public void CreateZip(string zipFilePath, string directoryPath)
        {
            ZipUtils.CreateZip(zipFilePath, directoryPath);
        }

        public void ExtractZip(string zipFilePath, string directoryPath)
        {
            ZipUtils.ExtractZip(zipFilePath, directoryPath);
        }

        public string JsonSerialize(object obj)
        {
            return TranslateUtils.JsonSerialize(obj);
        }

        public T JsonDeserialize<T>(string json, T defaultValue = default(T))
        {
            return TranslateUtils.JsonDeserialize(json, defaultValue);
        }

        public IAuthenticatedRequest GetAuthenticatedRequest(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("AuthenticatedRequest"))
            {
                return request.Properties["AuthenticatedRequest"] as IAuthenticatedRequest;
            }

            var authenticatedRequest = new AuthenticatedRequest(request);
            request.Properties["AuthenticatedRequest"] = authenticatedRequest;
            return authenticatedRequest;
        }
    }
}
