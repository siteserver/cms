using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
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
        string Language { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        bool RedisIsEnabled { get; }
        string RedisConnectionString { get; }
        IList<Menu> Menus { get; }
        PermissionsSettings Permissions { get; }
        string Encrypt(string inputString);
        string Decrypt(string inputString);
        void SaveSettings(string adminUrl, string homeUrl, string language, bool isProtectData, DatabaseType databaseType, string databaseConnectionString, bool redisIsEnabled, string redisConnectionString);
    }
}
