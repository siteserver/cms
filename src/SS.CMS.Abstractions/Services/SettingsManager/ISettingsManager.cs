using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

// ReSharper disable CheckNamespace
namespace SS.CMS.Abstractions
{
    public interface ISettingsManager
    {
        string ContentRootPath { get; }
        string WebRootPath { get; }
        string ProductVersion { get; }
        string PluginVersion { get; }
        string TargetFramework { get; }
        bool IsNightlyUpdate { get; }
        bool IsProtectData { get; }
        string AdminDirectory { get; }
        string HomeDirectory { get; }
        string SecurityKey { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        IDatabase Database { get; }
        string RedisConnectionString { get; }
        IRedis Redis { get; }
        IList<Menu> Menus { get; }
        PermissionsSettings Permissions { get; }
        string Encrypt(string inputString);
        string Decrypt(string inputString);
        Task SaveSettingsAsync(bool isNightlyUpdate, bool isProtectData, string adminDirectory, string homeDirectory, string securityKey, DatabaseType databaseType, string databaseConnectionString, string redisConnectionString);
    }
}
