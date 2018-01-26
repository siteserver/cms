using System;
using System.IO;
using System.Reflection;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Packaging;

namespace SiteServer.CMS.Core
{
    public class SystemManager
    {
        static SystemManager()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ssemblyName = assembly.GetName();
            var assemblyVersion = ssemblyName.Version;
            var version = assemblyVersion.ToString();
            if (StringUtils.EndsWith(version, ".0"))
            {
                version = version.Substring(0, version.Length - 2);
            }
            Version = version;
        }

        public static string Version { get; }

        public static void InstallDatabase(string adminName, string adminPassword)
        {
            CheckAndExecuteDatabase();

            if (!string.IsNullOrEmpty(adminName) && !string.IsNullOrEmpty(adminPassword))
            {
                RoleManager.CreatePredefinedRolesIfNotExists();

                var administratorInfo = new AdministratorInfo
                {
                    UserName = adminName,
                    Password = adminPassword
                };

                string errorMessage;
                AdminManager.CreateAdministrator(administratorInfo, out errorMessage);
                DataProvider.AdministratorsInRolesDao.AddUserToRole(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
            }
        }

        public static void UpdateDatabase()
        {
            CheckAndExecuteDatabase();
        }

        private static void CheckAndExecuteDatabase()
        {
            CacheUtils.ClearAll();

            foreach (var provider in DataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!DataProvider.DatabaseDao.IsTableExists(provider.TableName))
                {
                    DataProvider.DatabaseDao.CreateSystemTable(provider.TableName, provider.TableColumns);
                }
                else
                {
                    DataProvider.DatabaseDao.AlterSystemTable(provider.TableName, provider.TableColumns);
                }
            }

            var configInfo = DataProvider.ConfigDao.GetConfigInfo();
            if (configInfo == null)
            {
                configInfo = new ConfigInfo(true, Version, DateTime.Now, string.Empty);
                DataProvider.ConfigDao.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = Version;
                configInfo.IsInitialized = true;
                configInfo.UpdateDate = DateTime.Now;
                DataProvider.ConfigDao.Update(configInfo);
            }

            DataProvider.TableDao.CreateAllTableCollectionInfoIfNotExists();
        }

        public static bool IsNeedUpdate()
        {
            return !StringUtils.EqualsIgnoreCase(Version, DataProvider.ConfigDao.GetDatabaseVersion());
        }

        public static bool IsNeedInstall()
        {
            var isNeedInstall = !DataProvider.ConfigDao.IsInitialized();
            if (isNeedInstall)
            {
                isNeedInstall = !DataProvider.ConfigDao.IsInitialized();
            }
            return isNeedInstall;
        }

        public static bool DetermineRedirectToInstaller()
        {
            if (!IsNeedInstall()) return false;
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl("Installer"));
            return true;
        }

        public static bool UpdateSystem(string version, out string errorMessage)
        {
            try
            {
                var idWithVersion = $"{PackageUtils.PackageIdSsCms}.{version}";
                var packagePath = PathUtils.GetPackagesPath(idWithVersion);

                string nuspecPath;
                var metadata = GetSystemMetadataByDirectoryPath(packagePath, out nuspecPath, out errorMessage);
                if (metadata == null)
                {
                    return false;
                }

                DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFiles.DirectoryName), PathUtils.GetSiteFilesPath(string.Empty), true);
                DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteServer.DirectoryName), PathUtils.GetAdminDirectoryPath(string.Empty), true);
                DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.Bin.DirectoryName), PathUtils.GetBinDirectoryPath(string.Empty), true);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public static PackageMetadata GetSystemMetadataByDirectoryPath(string directoryPath, out string nuspecPath, out string errorMessage)
        {
            nuspecPath = string.Empty;

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
                errorMessage = "升级包配置文件不存在";
                return null;
            }

            PackageMetadata metadata;
            try
            {
                metadata = PackageUtils.GetPackageMetadata(nuspecPath);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            var pluginId = metadata.Id;

            if (string.IsNullOrEmpty(pluginId))
            {
                errorMessage = $"升级包配置文件 {nuspecPath} 不正确";
                return null;
            }

            errorMessage = string.Empty;
            return metadata;
        }
    }
}
