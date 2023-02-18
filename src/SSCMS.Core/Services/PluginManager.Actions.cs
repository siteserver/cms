using ICSharpCode.SharpZipLib.Zip;
using SSCMS.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMS.Core.Services
{
    public partial class PluginManager
    {
        public async Task InstallAsync(string userName, string name, string version, string downloadUrl)
        {
            var packagesPath = PathUtils.Combine(_settingsManager.ContentRootPath, DirectoryUtils.Packages);
            var pluginPath = PathUtils.Combine(DirectoryPath, $"{userName}.{name}");

            var zipFilePath = await DownloadExtensionAsync(packagesPath, userName, name, version, downloadUrl);

            var fz = new FastZip();
            fz.ExtractZip(zipFilePath, pluginPath, null);
        }

        private async Task<string> DownloadExtensionAsync(string packagesPath, string userName, string name, string version, string downloadUrl)
        {
            var fileName = $"{userName}.{name}.{version}";

            var directoryPath = PathUtils.Combine(packagesPath, fileName);
            var filePath = PathUtils.Combine(directoryPath, name + ".zip");
            if (FileUtils.IsFileExists(filePath))
            {
                return filePath;
            }

            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
            foreach (var directoryName in directoryNames.Where(directoryName => StringUtils.StartsWithIgnoreCase(directoryName, $"{userName}.{name}.")))
            {
                DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
            }

            await RestUtils.DownloadAsync(downloadUrl, filePath);

            //FileUtils.WriteText(filePath, string.Empty);
            //using (var writer = File.OpenWrite(filePath))
            //{
            //    var client = new RestClient(GetExtensionsDownloadUrl(userName, name, version));
            //    var request = new RestRequest();
            //    request.ResponseWriter = responseStream =>
            //    {
            //        using (responseStream)
            //        {
            //            responseStream.CopyTo(writer);
            //        }
            //    };
            //    client.DownloadData(request);
            //}

            //var repo = PackageRepositoryFactory.Default.CreateRepository(WebConfigUtils.IsNightlyUpdate
            //? MyGetPackageSource
            //: NuGetPackageSource);

            //var packageManager = new PackageManager(repo, packagesPath);

            ////Download and unzip the package
            //packageManager.InstallPackage(packageId, SemanticVersion.Parse(version), true, WebConfigUtils.IsNightlyUpdate);

            //ZipUtils.UnpackFilesByExtension(PathUtils.Combine(directoryPath, idWithVersion + ".nupkg"),
            //    directoryPath, ".nuspec");

            return filePath;
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
