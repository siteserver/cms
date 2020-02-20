using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Plugin;

namespace SiteServer.CMS.Core
{
    public static class SystemManager
    {
        public static async Task LoadSettingsAsync(string applicationPhysicalPath)
        {
            WebConfigUtils.Load(applicationPhysicalPath, PathUtils.Combine(applicationPhysicalPath, WebConfigUtils.WebConfigFileName));

            //await Caching.LoadCacheAsync();

            await PluginManager.LoadPluginsAsync(applicationPhysicalPath);

            try
            {
                ProductVersion = FileVersionInfo.GetVersionInfo(PathUtility.GetBinDirectoryPath("SiteServer.CMS.dll")).ProductVersion;
                PluginVersion = FileVersionInfo.GetVersionInfo(PathUtility.GetBinDirectoryPath("SiteServer.Abstractions.dll")).ProductVersion;

                if (Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                    .SingleOrDefault() is TargetFrameworkAttribute targetFrameworkAttribute)
                {
                    TargetFramework = targetFrameworkAttribute.FrameworkName;
                }

                EnvironmentVersion = Environment.Version.ToString();

                //DotNetVersion = FileVersionInfo.GetVersionInfo(typeof(Uri).Assembly.Location).ProductVersion;
            }
            catch
            {
                // ignored
            }
        }

        public static string ProductVersion { get; private set; }

        public static string PluginVersion { get; private set; }

        public static string TargetFramework { get; private set; }

        public static string EnvironmentVersion { get; private set; }
    }
}
