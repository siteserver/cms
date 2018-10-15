using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Version = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.CMS.dll")).ProductVersion;
                PluginVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.Plugin.dll")).ProductVersion;
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

        public static string Version { get; }

        public static string PluginVersion { get; }

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

        public static void CreateSiteServerTables()
        {
            foreach (var provider in DataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!DataProvider.DatabaseDao.IsTableExists(provider.TableName))
                {
                    DataProvider.DatabaseDao.CreateTable(provider.TableName, provider.TableColumns, out _, out _);
                }
                else
                {
                    DataProvider.DatabaseDao.AlterSystemTable(provider.TableName, provider.TableColumns);
                }
            }
        }

        public static void SyncContentTables()
        {
            var tableNameList = SiteManager.GetAllTableNameList();
            foreach (var tableName in tableNameList)
            {
                if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    DataProvider.DatabaseDao.CreateTable(tableName, DataProvider.ContentDao.TableColumns, out _, out _);
                }
                else
                {
                    DataProvider.DatabaseDao.AlterSystemTable(tableName, DataProvider.ContentDao.TableColumns, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        public static void UpdateConfigVersion()
        {
            var configInfo = DataProvider.ConfigDao.GetConfigInfo();
            if (configInfo == null)
            {
                configInfo = new ConfigInfo(0, true, Version, DateTime.Now, string.Empty);
                DataProvider.ConfigDao.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = Version;
                configInfo.IsInitialized = true;
                configInfo.UpdateDate = DateTime.Now;
                DataProvider.ConfigDao.Update(configInfo);
            }
        }

        public static void SyncDatabase()
        {
            CacheUtils.ClearAll();

            CreateSiteServerTables();

            SyncContentTables();

            UpdateConfigVersion();
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

        //public static bool DetermineRedirectToInstaller()
        //{
        //    if (!IsNeedInstall()) return false;
        //    PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl("Installer"));
        //    return true;
        //}
    }
}
