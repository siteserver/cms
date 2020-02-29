using System;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Packaging;

namespace SS.CMS.Services
{
    public partial class PluginManager
    {
        public bool UpdatePackage(string idWithVersion, PackageType packageType, out string errorMessage)
        {
            try
            {
                var packagePath = _pathManager.GetPackagesPath(idWithVersion);

                string nuspecPath;
                string dllDirectoryPath;
                var metadata = PackageUtils.GetPackageMetadataFromPackages(_pathManager, idWithVersion, out nuspecPath, out dllDirectoryPath, out errorMessage);
                if (metadata == null)
                {
                    return false;
                }

                if (packageType == PackageType.SsCms)
                {
                    var packageWebConfigPath = PathUtils.Combine(packagePath, Constants.ConfigFileName);
                    if (!FileUtils.IsFileExists(packageWebConfigPath))
                    {
                        errorMessage = $"升级包 {Constants.ConfigFileName} 文件不存在";
                        return false;
                    }

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
                            var destDllPath = _pathManager.GetBinDirectoryPath(fileName);
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
