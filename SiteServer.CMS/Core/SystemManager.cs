using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Datory;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class SystemManager
    {
        static SystemManager()
        {
            try
            {
                ProductVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.CMS.dll")).ProductVersion;
                PluginVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.Plugin.dll")).ProductVersion;

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

            //var ssemblyName = assembly.GetName();
            //var assemblyVersion = ssemblyName.Version;
            //var version = assemblyVersion.ToString();
            //if (StringUtils.EndsWith(version, ".0"))
            //{
            //    version = version.Substring(0, version.DataLength - 2);
            //}
            //Version = version;
        }

        public static string ProductVersion { get; }

        public static string PluginVersion { get; }

        public static string TargetFramework { get; }

        public static string EnvironmentVersion { get; }

        public static void InstallDatabase(string adminName, string adminPassword)
        {
            SyncDatabase();

            if (!string.IsNullOrEmpty(adminName) && !string.IsNullOrEmpty(adminPassword))
            {
                var administratorInfo = new AdministratorInfo
                {
                    UserName = adminName,
                    Password = adminPassword
                };

                DataProvider.AdministratorDao.Insert(administratorInfo, out _);
                DataProvider.AdministratorsInRolesDao.AddUserToRole(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
            }
        }

        public static void SyncSystemTables()
        {
            foreach (var repository in DataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!DatoryUtils.IsTableExists(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, repository.TableName))
                {
                    TableColumnManager.CreateTable(repository.TableName, repository.TableColumns, string.Empty, false, out _);
                }
                else
                {
                    TableColumnManager.AlterTable(repository.TableName, repository.TableColumns, string.Empty);
                }
            }
        }

        public static void SyncContentTables()
        {
            var tableNameList = SiteManager.GetAllTableNameList();
            foreach (var tableName in tableNameList)
            {
                if (!DatoryUtils.IsTableExists(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName))
                {
                    TableColumnManager.CreateTable(tableName, DataProvider.ContentDao.TableColumns, string.Empty, true, out _);
                }
                else
                {
                    TableColumnManager.AlterTable(tableName, DataProvider.ContentDao.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        public static void UpdateConfigVersion()
        {
            var configInfo = DataProvider.ConfigDao.GetConfigInfo();
            if (configInfo == null)
            {
                configInfo = new ConfigInfo
                {
                    Initialized = true,
                    DatabaseVersion = ProductVersion,
                    UpdateDate = DateTime.Now
                };
                DataProvider.ConfigDao.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = ProductVersion;
                configInfo.Initialized = true;
                configInfo.UpdateDate = DateTime.Now;
                DataProvider.ConfigDao.Update(configInfo);
            }
        }

        public static void SyncDatabase()
        {
            CacheUtils.ClearAll();

            SyncSystemTables();

            SyncContentTables();

            UpdateConfigVersion();
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

        //public static bool DetermineRedirectToInstaller()
        //{
        //    if (!IsNeedInstall()) return false;
        //    PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl("Installer"));
        //    return true;
        //}
    }
}
