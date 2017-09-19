using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface IFilesApi
    {
        void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls);

        string GetUploadFilePath(int publishmentSystemId, string filePath);

        string GetTemporaryFilesPath(string relatedPath);
    }
}
