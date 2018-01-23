using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Apis
{
    public class FilesApi : IFilesApi
    {
        private readonly IMetadata _metadata;

        public FilesApi(IMetadata metadata)
        {
            _metadata = metadata;
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

        public string GetUploadFilePath(int siteId, string relatedPath)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(siteInfo, PathUtils.GetExtension(relatedPath));
            var localFileName = PathUtility.GetUploadFileName(siteInfo, relatedPath);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }

        public string GetTemporaryFilesPath(string relatedPath)
        {
            return PathUtils.GetTemporaryFilesPath(relatedPath);
        }

        public string GetPluginPath(string relatedPath)
        {
            var path = PathUtils.Combine(PathUtils.GetPluginPath(_metadata.Id), relatedPath);
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public string GetPluginUrl(string relatedUrl = "")
        {
            return PageUtility.GetSiteFilesUrl(PageUtility.InnerApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string GetApiUrl(string relatedUrl = "")
        {
            return PageUtils.Combine(PageUtility.OuterApiUrl, relatedUrl);
        }

        public string GetApiPluginUrl(string relatedUrl = "")
        {
            return PageUtility.GetSiteFilesUrl(PageUtility.OuterApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string GetApiPluginJsonUrl(string name = "", string id = "")
        {
            return Controllers.Json.PluginJsonApi.GetUrl(PageUtility.OuterApiUrl, _metadata.Id, name, id);
        }

        public string GetApiPluginHttpUrl(string name = "", string id = "")
        {
            return Controllers.Http.PluginHttpApi.GetUrl(PageUtility.OuterApiUrl, _metadata.Id, name, id);
        }

        public string GetSiteUrl(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetSiteUrl(siteInfo, false);
        }

        public string GetSiteUrl(int siteId, string relatedUrl)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetSiteUrl(siteInfo, relatedUrl, false);
        }

        public string GetSiteUrlByFilePath(string filePath)
        {
            var siteId = SiteApi.Instance.GetSiteIdByFilePath(filePath);
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetSiteUrlByPhysicalPath(siteInfo, filePath, false);
        }

        public string GetRootUrl(string relatedUrl)
        {
            return PageUtils.GetRootUrl(relatedUrl);
        }

        public string GetAdminDirectoryUrl(string relatedUrl)
        {
            return PageUtils.GetAdminDirectoryUrl(relatedUrl);
        }

        public string GetChannelUrl(int siteId, int channelId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetChannelUrl(siteInfo, ChannelManager.GetChannelInfo(siteId, channelId), false);
        }

        public string GetContentUrl(int siteId, int channelId, int contentId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetContentUrl(siteInfo, ChannelManager.GetChannelInfo(siteId, channelId), contentId, false);
        }
    }
}
