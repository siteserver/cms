using System;
using System.IO;
using System.Linq;
using NuGet.Packaging;
using SS.CMS.Core.Packaging;
using SS.CMS.Enums;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public IPackageMetadata GetPackageMetadataFromPluginDirectory(string directoryName, out string errorMessage)
        {
            PackageMetadata metadata = null;

            var nuspecPath = _pathManager.GetPluginNuspecPath(directoryName);
            if (FileUtils.IsFileExists(nuspecPath))
            {
                try
                {
                    metadata = GetPackageMetadata(nuspecPath);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return null;
                }
            }

            if (string.IsNullOrEmpty(metadata?.Id))
            {
                metadata = new PackageMetadata(directoryName);
            }

            errorMessage = string.Empty;
            return metadata;
        }

        public IPackageMetadata GetPackageMetadataFromPackages(string directoryName, out string nuspecPath, out string dllDirectoryPath, out string errorMessage)
        {
            nuspecPath = string.Empty;
            dllDirectoryPath = string.Empty;
            errorMessage = string.Empty;

            var directoryPath = _pathManager.GetPackagesPath(directoryName);

            foreach (var filePath in DirectoryUtils.GetFilePaths(directoryPath))
            {
                if (StringUtils.EqualsIgnoreCase(Path.GetExtension(filePath), ".nuspec"))
                {
                    nuspecPath = filePath;
                    break;
                }
            }

            if (string.IsNullOrEmpty(nuspecPath))
            {
                errorMessage = "配置文件不存在";
                return null;
            }

            PackageMetadata metadata;
            try
            {
                metadata = GetPackageMetadata(nuspecPath);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            var packageId = metadata.Id;

            if (string.IsNullOrEmpty(packageId))
            {
                errorMessage = $"配置文件 {nuspecPath} 不正确";
                return null;
            }

            if (!StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
            {
                dllDirectoryPath = FindDllDirectoryPath(directoryPath);

                //if (!FileUtils.IsFileExists(PathUtils.Combine(dllDirectoryPath, packageId + ".dll")))
                //{
                //    errorMessage = $"插件可执行文件 {packageId}.dll 不存在";
                //    return null;
                //}
            }

            return metadata;
        }

        //https://docs.microsoft.com/en-us/nuget/schema/target-frameworks#supported-frameworks
        private string FindDllDirectoryPath(string packageDirectoryPath)
        {
            var dllDirectoryPath = string.Empty;

            foreach (var dirName in DirectoryUtils.GetDirectoryNames(PathUtils.Combine(packageDirectoryPath, "lib")))
            {
                if (StringUtils.StartsWithIgnoreCase(dirName, "net45") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net451") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net452") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net46") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net461") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net462"))
                {
                    dllDirectoryPath = PathUtils.Combine(packageDirectoryPath, "lib", dirName);
                    break;
                }
            }
            if (string.IsNullOrEmpty(dllDirectoryPath))
            {
                dllDirectoryPath = PathUtils.Combine(packageDirectoryPath, "lib");
            }

            return dllDirectoryPath;
        }

        private PackageMetadata GetPackageMetadata(string configPath)
        {
            var nuspecReader = new NuspecReader(configPath);

            var rawMetadata = nuspecReader.GetMetadata();
            if (rawMetadata == null || !rawMetadata.Any()) return null;

            return PackageMetadata.FromNuspecReader(nuspecReader);
        }

        public void DownloadPackage(string packageId, string version)
        {
            var packagesPath = _pathManager.GetPackagesPath();
            var idWithVersion = $"{packageId}.{version}";
            var directoryPath = PathUtils.Combine(packagesPath, idWithVersion);

            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                if (FileUtils.IsFileExists(PathUtils.Combine(directoryPath, $"{idWithVersion}.nupkg")) && FileUtils.IsFileExists(PathUtils.Combine(directoryPath, $"{packageId}.nuspec")))
                {
                    return;
                }
            }

            var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
            foreach (var directoryName in directoryNames)
            {
                if (StringUtils.StartsWithIgnoreCase(directoryName, $"{packageId}."))
                {
                    DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
                }
            }

            if (StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var localFilePath = PathUtils.Combine(directoryPath, idWithVersion + ".nupkg");
                HttpClientUtils.SaveRemoteFileToLocal(
                    $"https://api.siteserver.cn/downloads/update/{version}", localFilePath);

                ZipUtils.ExtractZip(localFilePath, directoryPath);
            }
            else
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var localFilePath = PathUtils.Combine(directoryPath, idWithVersion + ".nupkg");

                HttpClientUtils.SaveRemoteFileToLocal(
                    $"https://api.siteserver.cn/downloads/package/{packageId}/{version}", localFilePath);

                ZipUtils.ExtractZip(localFilePath, directoryPath);

                //var repo = PackageRepositoryFactory.Default.CreateRepository(WebConfigUtils.IsNightlyUpdate
                //? MyGetPackageSource
                //: NuGetPackageSource);

                //var packageManager = new PackageManager(repo, packagesPath);

                ////Download and unzip the package
                //packageManager.InstallPackage(packageId, SemanticVersion.Parse(version), true, WebConfigUtils.IsNightlyUpdate);
            }

            //ZipUtils.UnpackFilesByExtension(PathUtils.Combine(directoryPath, idWithVersion + ".nupkg"),
            //    directoryPath, ".nuspec");
        }

        public bool UpdatePackage(string idWithVersion, PackageType packageType, out string errorMessage)
        {
            try
            {
                var packagePath = _pathManager.GetPackagesPath(idWithVersion);

                string nuspecPath;
                string dllDirectoryPath;
                var metadata = GetPackageMetadataFromPackages(idWithVersion, out nuspecPath, out dllDirectoryPath, out errorMessage);
                if (metadata == null)
                {
                    return false;
                }

                if (packageType == PackageType.SsCms)
                {
                    // var packageWebConfigPath = PathUtils.Combine(packagePath, AppSettings.WebConfigFileName);
                    // if (!FileUtils.IsFileExists(packageWebConfigPath))
                    // {
                    //     errorMessage = $"升级包 {AppSettings.WebConfigFileName} 文件不存在";
                    //     return false;
                    // }

                    // AppSettings.UpdateWebConfig(packageWebConfigPath, AppSettings.IsProtectData,
                    //     AppSettings.DatabaseType, AppSettings.ConnectionString, AppSettings.ApiPrefix, AppSettings.AdminDirectory, AppSettings.HomeDirectory,
                    //     AppSettings.SecretKey, AppSettings.IsNightlyUpdate);

                    //DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName),
                    //    PathUtils.GetSiteFilesPath(string.Empty), true);
                    //DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName),
                    //    PathUtils.GetAdminDirectoryPath(string.Empty), true);
                    //DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName),
                    //    PathUtils.GetBinDirectoryPath(string.Empty), true);
                    //FileUtils.CopyFile(packageWebConfigPath,
                    //    PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.WebConfigFileName),
                    //    true);
                }
                else if (packageType == PackageType.Plugin)
                {
                    var pluginPath = _pathManager.GetPluginPath(metadata.Id);
                    DirectoryUtils.CreateDirectoryIfNotExists(pluginPath);

                    DirectoryUtils.Copy(PathUtils.Combine(packagePath, "content"), pluginPath, true);
                    DirectoryUtils.Copy(dllDirectoryPath, PathUtils.Combine(pluginPath, "Bin"), true);

                    //var dependencyPackageDict = GetDependencyPackages(metadata);
                    //foreach (var dependencyPackageId in dependencyPackageDict.Keys)
                    //{
                    //    var dependencyPackageVersion = dependencyPackageDict[dependencyPackageId];
                    //    var dependencyDdlDirectoryPath =
                    //        FindDllDirectoryPath(
                    //            PathUtils.GetPackagesPath($"{dependencyPackageId}.{dependencyPackageVersion}"));
                    //    DirectoryUtils.Copy(dependencyDdlDirectoryPath, PathUtils.Combine(pluginPath, "Bin"), true);
                    //}

                    var configFilelPath = PathUtils.Combine(pluginPath, $"{metadata.Id}.nuspec");
                    FileUtils.CopyFile(nuspecPath, configFilelPath, true);

                    ClearCache();
                }
                else if (packageType == PackageType.Library)
                {
                    var fileNames = DirectoryUtils.GetFileNames(dllDirectoryPath);
                    foreach (var fileName in fileNames)
                    {
                        if (StringUtils.EndsWithIgnoreCase(fileName, ".dll"))
                        {
                            var sourceDllPath = PathUtils.Combine(dllDirectoryPath, fileName);
                            var destDllPath = PathUtils.GetBinDirectoryPath(fileName);
                            if (!FileUtils.IsFileExists(destDllPath))
                            {
                                FileUtils.CopyFile(sourceDllPath, destDllPath, false);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }
    }
}