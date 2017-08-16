using System;
using System.Collections.Generic;
using SiteServer.Plugin.Data;

namespace SiteServer.Plugin
{
    /// <summary>
    /// Public APIs that plugin can use
    /// </summary>
    public interface IPublicApi
    {
        string DatabaseType { get; }

        string ConnectionString { get; }

        IDbHelper DbHelper { get; }

        bool SetGlobalConfig(string name, object config);

        T GetGlobalConfig<T>(string name);

        bool RemoveGlobalConfig(string name);

        bool SetConfig(int siteId, string name, object config);

        T GetConfig<T>(int siteId, string name);

        bool RemoveConfig(int siteId, string name);

        int GetSiteIdByFilePath(string path);

        string GetSiteDirectoryPath(int siteId);

        List<int> GetSiteIds();

        void AddErrorLog(Exception ex);

        void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls);

        bool IsAuthorized();

        bool IsSiteAuthorized(int siteId);

        string GetUploadFilePath(int siteId, string filePath);

        string GetUrlByFilePath(string filePath);

        string GetPluginUrl(int siteId, string relatedUrl = "");

        string GetPluginRestfulApiUrl(int siteId, string action = "", int id = 0);

        string GetPluginHttpApiUrl(int siteId, string action = "", int id = 0);

        IPublishmentSystemInfo GetPublishmentSystemInfo(int siteId);

        INodeInfo GetNodeInfo(int siteId, int channelId);

        IContentInfo GetContentInfo(int siteId, int channelId, int contentId);
    }
}
