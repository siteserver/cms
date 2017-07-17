using System;
using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// Public APIs that plugin can use
    /// </summary>
    public interface IPublicApi
    {
        IDbHelper DbHelper { get; }

        bool SetConfig(string name, object config);

        T GetConfig<T>(string name);

        bool RemoveConfig(string name);

        bool SetSiteConfig(int siteId, string name, object config);

        T GetSiteConfig<T>(int siteId, string name);

        bool RemoveSiteConfig(int siteId, string name);

        int GetSiteIdByFilePath(string path);

        string GetSiteDirectoryPath(int siteId);

        List<int> GetSiteIds();

        void AddErrorLog(Exception ex);

        void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls);

        bool IsAuthorized();

        bool IsSiteAuthorized(int siteId);
    }
}
