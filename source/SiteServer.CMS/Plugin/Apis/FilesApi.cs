using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Apis
{
    public class FilesApi : IFilesApi
    {
        private FilesApi() { }

        public static FilesApi Instance { get; } = new FilesApi();

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

        public string GetUploadFilePath(int publishmentSystemId, string fileName)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, PathUtils.GetExtension(fileName));
            var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }

        public string GetTemporaryFilesPath(string relatedPath)
        {
            return PathUtils.GetTemporaryFilesPath(relatedPath);
        }
    }
}
