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

        public void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls)
        {
            if (sourcePublishmentSystemId == targetPublishmentSystemId) return;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(sourcePublishmentSystemId);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
            if (publishmentSystemInfo == null || targetPublishmentSystemInfo == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    FileUtility.MoveFile(publishmentSystemInfo, targetPublishmentSystemInfo, relatedUrl);
                }
            }
        }

        public void AddWaterMark(int publishmentSystemId, string filePath)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            FileUtility.AddWaterMark(publishmentSystemInfo, filePath);
        }

        public string GetUploadFilePath(int publishmentSystemId, string relatedPath)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, PathUtils.GetExtension(relatedPath));
            var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, relatedPath);
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

        public string GetPublishmentSystemUrl(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrl(publishmentSystemInfo, false);
        }

        public string GetPublishmentSystemUrl(int publishmentSystemId, string relatedUrl)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrl(publishmentSystemInfo, relatedUrl, false);
        }

        public string GetPublishmentSystemUrlByFilePath(string filePath)
        {
            var publishmentSystemId = PublishmentSystemApi.Instance.GetPublishmentSystemIdByFilePath(filePath);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, filePath, false);
        }

        public string GetRootUrl(string relatedUrl)
        {
            return PageUtils.GetRootUrl(relatedUrl);
        }

        public string GetAdminDirectoryUrl(string relatedUrl)
        {
            return PageUtils.GetAdminDirectoryUrl(relatedUrl);
        }

        public string GetChannelUrl(int publishmentSystemId, int channelId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetChannelUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemId, channelId), false);
        }

        public string GetContentUrl(int publishmentSystemId, int channelId, int contentId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetContentUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemId, channelId), contentId, false);
        }
    }
}
