using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Settings;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Services
{
    public interface ISettingsManager
    {
        string ContentRootPath { get; }

        string WebRootPath { get; }

        bool IsProtectData { get; }
        string ApiPrefix { get; }

        /// <summary>
        /// 管理后台文件夹名称。
        /// </summary>
        string AdminDirectory { get; }

        /// <summary>
        /// 用户中心文件夹名称。
        /// </summary>
        string HomeDirectory { get; }
        string SecretKey { get; }
        bool IsNightlyUpdate { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        string RedisConnectionString { get; }

        NavSettings NavSettings { get; }
        ConfigInfo ConfigInfo { get; }

        bool IsChanged { set; }

        string Encrypt(string inputString);

        string Decrypt(string inputString);
    }
}
