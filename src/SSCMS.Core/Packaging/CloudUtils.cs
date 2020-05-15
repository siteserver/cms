using System.IO;
using System.Linq;
using RestSharp;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Packaging
{
    public static class CloudUtils
    {
        public static class Www
        {
            public const string Host = "http://www.sscms.com";
        }

        public static class Dl
        {
            private const string Host = "http://dl.sscms.com";

            public static string GetCmsDownloadName(string version)
            {
                return $"sscms-{version}-{Constants.OperatingSystem}";
            }

            public static string GetCmsDownloadUrl(string version)
            {
                return $"{Host}/cms/{version}/{GetCmsDownloadName(version)}.zip";
            }

            public static string GetPluginsDownloadUrl(string pluginId, string version)
            {
                return $"{Host}/plugins/{GetPluginsDownloadName(pluginId, version)}.zip";
            }

            public static string GetPluginsDownloadName(string pluginId, string version)
            {
                return $"{pluginId}.{version}";
            }

            public static void DownloadCms(IPathManager pathManager, string version)
            {
                if (IsCmsDownload(pathManager, version))
                {
                    return;
                }

                var packagesPath = pathManager.GetPackagesPath();
                var name = GetCmsDownloadName(version);

                var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
                foreach (var directoryName in directoryNames.Where(directoryName => StringUtils.StartsWithIgnoreCase(directoryName, "sscms-")))
                {
                    DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
                }

                var directoryPath = PathUtils.Combine(packagesPath, name);
                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var filePath = PathUtils.Combine(directoryPath, $"{GetCmsDownloadName(version)}.zip");
                FileUtils.WriteText(filePath, string.Empty);
                using (var writer = File.OpenWrite(filePath))
                {
                    var client = new RestClient(GetCmsDownloadUrl(version));
                    var request = new RestRequest();
                    request.ResponseWriter = responseStream =>
                    {
                        using (responseStream)
                        {
                            responseStream.CopyTo(writer);
                        }
                    };
                    client.DownloadData(request);
                }

                ZipUtils.ExtractZip(filePath, directoryPath);
            }

            public static bool IsCmsDownload(IPathManager pathManager, string version)
            {
                var packagesPath = pathManager.GetPackagesPath();
                var name = GetCmsDownloadName(version);

                var directoryPath = PathUtils.Combine(packagesPath, name);

                if (!DirectoryUtils.IsDirectoryExists(directoryPath))
                {
                    return false;
                }

                if (!FileUtils.IsFileExists(PathUtils.Combine(directoryPath, $"{name}.zip")))
                {
                    return false;
                }

                var fileNames = DirectoryUtils.GetFileNames(directoryPath);

                return fileNames.Count > 1;
            }

            public static void DownloadPlugin(IPathManager pathManager, string pluginId, string version)
            {
                if (IsPluginDownload(pathManager, pluginId, version))
                {
                    return;
                }

                var name = GetPluginsDownloadName(pluginId, version);

                var packagesPath = pathManager.GetPackagesPath();
                var directoryPath = PathUtils.Combine(packagesPath, name);

                var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
                foreach (var directoryName in directoryNames.Where(directoryName => StringUtils.StartsWithIgnoreCase(directoryName, $"{pluginId}.")))
                {
                    DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
                }

                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var filePath = PathUtils.Combine(directoryPath, name + ".zip");
                FileUtils.WriteText(filePath, string.Empty);
                using (var writer = File.OpenWrite(filePath))
                {
                    var client = new RestClient(GetCmsDownloadUrl(version));
                    var request = new RestRequest();
                    request.ResponseWriter = responseStream =>
                    {
                        using (responseStream)
                        {
                            responseStream.CopyTo(writer);
                        }
                    };
                    client.DownloadData(request);
                }

                ZipUtils.ExtractZip(filePath, directoryPath);

                //var repo = PackageRepositoryFactory.Default.CreateRepository(WebConfigUtils.IsNightlyUpdate
                //? MyGetPackageSource
                //: NuGetPackageSource);

                //var packageManager = new PackageManager(repo, packagesPath);

                ////Download and unzip the package
                //packageManager.InstallPackage(packageId, SemanticVersion.Parse(version), true, WebConfigUtils.IsNightlyUpdate);

                //ZipUtils.UnpackFilesByExtension(PathUtils.Combine(directoryPath, idWithVersion + ".nupkg"),
                //    directoryPath, ".nuspec");
            }

            public static bool IsPluginDownload(IPathManager pathManager, string pluginId, string version)
            {
                var name = GetPluginsDownloadName(pluginId, version);
                var packagesPath = pathManager.GetPackagesPath();
                var directoryPath = PathUtils.Combine(packagesPath, name);

                if (!DirectoryUtils.IsDirectoryExists(directoryPath))
                {
                    return false;
                }

                if (!FileUtils.IsFileExists(PathUtils.Combine(directoryPath, $"{name}.zip")))
                {
                    return false;
                }

                var fileNames = DirectoryUtils.GetFileNames(directoryPath);

                return fileNames.Count > 1;
            }
        }
    }
}
