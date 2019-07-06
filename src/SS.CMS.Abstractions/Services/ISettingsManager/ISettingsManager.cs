using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public interface ISettingsManager
    {
        string ContentRootPath { get; }
        string WebRootPath { get; }
        string ProductVersion { get; }
        string PluginVersion { get; }
        string TargetFramework { get; }
        bool IsProtectData { get; }
        string AdminUrl { get; }
        string HomeUrl { get; }
        string SecurityKey { get; }
        bool IsNightlyUpdate { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        CacheType CacheType { get; }
        string CacheConnectionString { get; }
        IList<Menu> Menus { get; }
        PermissionsSettings Permissions { get; }
        string Encrypt(string inputString);
        string Decrypt(string inputString);
        Task SaveSettingsAsync(string adminUrl, string homeUrl, bool isNightlyUpdate, bool isProtectData, string securityKey, DatabaseType databaseType, string databaseConnectionString, CacheType cacheType, string cacheConnectionString);
    }
}
