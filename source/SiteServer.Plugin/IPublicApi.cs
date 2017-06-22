using System;

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

        /// <summary>
        /// Set a new option.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetOption(string option, string value);

        string GetOption(string option);

        bool RemoveOption(string option);

        bool SetSiteOption(int siteId, string option, string value);

        string GetSiteOption(int siteId, string option);

        bool RemoveSiteOption(int siteId, string option);

        int GetSiteIdByFilePath(string path);

        string GetSiteDirectoryPath(int siteId);

        void AddErrorLog(Exception ex);
    }
}
