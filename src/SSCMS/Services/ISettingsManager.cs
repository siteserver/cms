using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

// ReSharper disable CheckNamespace
namespace SSCMS
{
    public interface ISettingsManager
    {
        string ContentRootPath { get; }
        string WebRootPath { get; }
        string AppVersion { get; }
        string SdkVersion { get; }
        string TargetFramework { get; }
        bool IsNightlyUpdate { get; }
        bool IsProtectData { get; }
        string SecurityKey { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        IDatabase Database { get; }
        string RedisConnectionString { get; }
        IRedis Redis { get; }
        IList<Menu> Menus { get; }
        PermissionsSettings Permissions { get; }
        string Encrypt(string inputString, string securityKey = null);
        string Decrypt(string inputString, string securityKey = null);
        Task SaveSettingsAsync(bool isNightlyUpdate, bool isProtectData, string securityKey, DatabaseType databaseType, string databaseConnectionString, string redisConnectionString);
    }
}
