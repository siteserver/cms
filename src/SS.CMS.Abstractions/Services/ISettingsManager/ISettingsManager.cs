using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Settings;

namespace SS.CMS.Services.ISettingsManager
{
    public interface ISettingsManager
    {
        string ContentRootPath { get; }

        string WebRootPath { get; }

        string ProductVersion { get; }

        string PluginVersion { get; }

        string TargetFramework { get; }

        string EnvironmentVersion { get; }

        bool IsProtectData { get; }
        string ApiPrefix { get; }

        /// <summary>
        /// 管理后台文件夹名称。
        /// </summary>
        string AdminPrefix { get; }

        /// <summary>
        /// 用户中心文件夹名称。
        /// </summary>
        string HomePrefix { get; }
        string SecretKey { get; }
        bool IsNightlyUpdate { get; }
        string Language { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        string RedisConnectionString { get; }

        IList<Menu> Menus { get; }
        PermissionsSettings Permissions { get; }

        string Encrypt(string inputString);

        string Decrypt(string inputString);
    }
}
