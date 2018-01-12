using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface IFilesApi
    {
        void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls);

        void AddWaterMark(int publishmentSystemId, string filePath);

        string GetUploadFilePath(int publishmentSystemId, string relatedPath);

        string GetTemporaryFilesPath(string relatedPath);

        string GetPluginPath(string relatedPath);

        string GetPluginUrl(string relatedUrl = "");

        string GetApiUrl(string relatedUrl = "");

        string GetApiPluginUrl(string relatedUrl = "");

        string GetApiPluginJsonUrl(string action = "", string id = "");

        string GetApiPluginHttpUrl(string action = "", string id = "");

        string GetPublishmentSystemUrl(int publishmentSystemId);

        string GetPublishmentSystemUrl(int publishmentSystemId, string relatedUrl);

        string GetPublishmentSystemUrlByFilePath(string filePath);

        string GetChannelUrl(int publishmentSystemId, int channelId);

        string GetContentUrl(int publishmentSystemId, int channelId, int contentId);

        string GetRootUrl(string relatedUrl);

        string GetAdminDirectoryUrl(string relatedUrl);
    }
}
