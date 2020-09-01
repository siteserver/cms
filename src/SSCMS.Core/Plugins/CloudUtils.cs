using System.IO;
using System.Linq;
using RestSharp;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public static class CloudUtils
    {
        public static class Www
        {
            public const string Host = "https://sscms.com";

            public static string GetPluginUrl(string pluginId)
            {
                return PageUtils.Combine(Host, $"/plugins/plugin.html?id={pluginId}");
            }
        }

        public static class Api
        {
            private const string Host = "https://api.sscms.com";
            // private const string Host = "http://localhost:81";

            public static string GetCliUrl(string relatedUrl)
            {
                return PageUtils.Combine(Host, "v7/cli", relatedUrl);
            }
        }

        public static class Dl
        {
            private const string Host = "https://dl.sscms.com";

            public static string GetCmsDownloadName(string osArchitecture, string version)
            {
                return $"sscms-{version}-{osArchitecture}";
            }

            public static string GetCmsDownloadUrl(string osArchitecture, string version)
            {
                return $"{Host}/cms/{version}/{GetCmsDownloadName(osArchitecture, version)}.zip";
            }

            public static string GetPluginsDownloadUrl(string pluginId, string version)
            {
                var publisher = StringUtils.GetFirstOfStringCollection(pluginId, '.');
                return $"{Host}/plugins/{publisher}/{GetPluginsDownloadName(pluginId, version)}.zip";
            }

            public static string GetPluginsDownloadName(string pluginId, string version)
            {
                return $"{pluginId}.{version}";
            }

            public static string DownloadCms(IPathManager pathManager, string osArchitecture, string version)
            {
                var packagesPath = pathManager.GetPackagesPath();
                var name = GetCmsDownloadName(osArchitecture, version);
                var directoryPath = PathUtils.Combine(packagesPath, name);

                //if (IsCmsDownload(pathManager, osArchitecture, version))
                //{
                //    return directoryPath;
                //}

                var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
                foreach (var directoryName in directoryNames.Where(directoryName => StringUtils.StartsWithIgnoreCase(directoryName, "sscms-")))
                {
                    DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
                }

                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var filePath = PathUtils.Combine(packagesPath, $"{GetCmsDownloadName(osArchitecture, version)}.zip");
                FileUtils.WriteText(filePath, string.Empty);
                using (var writer = File.OpenWrite(filePath))
                {
                    var client = new RestClient(GetCmsDownloadUrl(osArchitecture, version));
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

                pathManager.ExtractZip(filePath, directoryPath);

                FileUtils.DeleteFileIfExists(filePath);

                return directoryPath;
            }

            public static bool IsCmsDownload(IPathManager pathManager, string osArchitecture, string version)
            {
                var packagesPath = pathManager.GetPackagesPath();
                var name = GetCmsDownloadName(osArchitecture, version);

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

            public static string DownloadPlugin(string packagesPath, string pluginId, string version)
            {
                var name = GetPluginsDownloadName(pluginId, version);

                var directoryPath = PathUtils.Combine(packagesPath, name);
                var filePath = PathUtils.Combine(directoryPath, name + ".zip");
                if (FileUtils.IsFileExists(filePath))
                {
                    return filePath;
                }

                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
                foreach (var directoryName in directoryNames.Where(directoryName => StringUtils.StartsWithIgnoreCase(directoryName, $"{pluginId}.")))
                {
                    DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
                }

                FileUtils.WriteText(filePath, string.Empty);
                using (var writer = File.OpenWrite(filePath))
                {
                    var client = new RestClient(GetPluginsDownloadUrl(pluginId, version));
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
        }
    }
}
