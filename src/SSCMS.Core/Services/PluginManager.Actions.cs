using ICSharpCode.SharpZipLib.Zip;
using SSCMS.Core.Utils;
using SSCMS.Utils;
using System.Threading.Tasks;

namespace SSCMS.Core.Services
{
    public partial class PluginManager
    {
        public async Task InstallAsync(string userName, string name, string version)
        {
            var packagesPath = PathUtils.Combine(_settingsManager.ContentRootPath, DirectoryUtils.Packages);
            var pluginPath = PathUtils.Combine(DirectoryPath, $"{userName}.{name}");

            var zipFilePath = await CloudUtils.Dl.DownloadExtensionAsync(packagesPath, userName, name, version);

            var fz = new FastZip();
            fz.ExtractZip(zipFilePath, pluginPath, null);
        }

        public void UnInstall(string pluginId)
        {
            var pluginPath = PathUtils.Combine(DirectoryPath, pluginId);
            try
            {
                DirectoryUtils.DeleteDirectoryIfExists(pluginPath);
            }
            catch
            {
                DirectoryUtils.DeleteDirectoryIfExists(pluginPath);
            }
        }
    }
}
