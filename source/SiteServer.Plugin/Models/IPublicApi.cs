using System;
using System.Collections.Generic;

namespace SiteServer.Plugin.Models
{
    /// <summary>
    /// Public APIs that plugin can use
    /// </summary>
    public interface IPublicApi
    {
        string DatabaseType { get; }

        string ConnectionString { get; }

        IDbHelper DbHelper { get; }

        bool SetGlobalConfig(object config);

        bool SetGlobalConfig(string name, object config);

        T GetGlobalConfig<T>(string name = "");

        bool RemoveGlobalConfig(string name = "");

        bool SetConfig(int publishmentSystemId, string name, object config);

        bool SetConfig(int publishmentSystemId, object config);

        T GetConfig<T>(int publishmentSystemId, string name = "");

        bool RemoveConfig(int publishmentSystemId, string name = "");

        int GetPublishmentSystemIdByFilePath(string path);

        string GetPublishmentSystemPath(int publishmentSystemId);

        List<int> GetPublishmentSystemIds();

        void AddErrorLog(Exception ex);

        void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls);

        bool IsAuthorized();

        bool IsAuthorized(int publishmentSystemId);

        string GetUploadFilePath(int publishmentSystemId, string filePath);

        string GetUrlByFilePath(string filePath);

        string GetPluginPageUrl(int publishmentSystemId, string relatedUrl = "");

        string GetPluginJsonApiUrl(int publishmentSystemId, string action = "", int id = 0);

        string GetPluginHttpApiUrl(int publishmentSystemId, string action = "", int id = 0);

        IPublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId);

        INodeInfo GetNodeInfo(int publishmentSystemId, int channelId);

        IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId);
    }
}
