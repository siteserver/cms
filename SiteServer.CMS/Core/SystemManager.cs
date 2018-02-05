using System;
using System.Diagnostics;
using System.Reflection;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public class SystemManager
    {
        static SystemManager()
        {
            Version = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.CMS.dll")).ProductVersion;
            PluginVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.Plugin.dll")).ProductVersion;

            //var ssemblyName = assembly.GetName();
            //var assemblyVersion = ssemblyName.Version;
            //var version = assemblyVersion.ToString();
            //if (StringUtils.EndsWith(version, ".0"))
            //{
            //    version = version.Substring(0, version.Length - 2);
            //}
            //Version = version;
        }

        public static string Version { get; }

        public static string PluginVersion { get; }

        public static void InstallDatabase(string adminName, string adminPassword)
        {
            SyncDatabase();

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

        public static void SyncDatabase()
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
    }
}
