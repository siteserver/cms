using System;
using System.Diagnostics;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
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

        public const string ApiVersion = "v1";

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

                DataProvider.Administrator.Insert(administratorInfo, out _);
                DataProvider.AdministratorsInRoles.AddUserToRole(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
            }
        }

        public static void SyncSystemTables()
        {
            foreach (var repository in DataProvider.AllRepositories)
            {
                if (string.IsNullOrEmpty(repository.TableName) || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

                if (!DatabaseApi.Instance.IsTableExists(repository.TableName))
                {
                    DatabaseApi.Instance.CreateTable(repository.TableName, repository.TableColumns, string.Empty, false, out _, out _);
                }
                else
                {
                    DatabaseApi.Instance.AlterTable(repository.TableName, repository.TableColumns, string.Empty);
                }
            }
        }

        public static void SyncContentTables()
        {
            var tableNameList = SiteManager.GetAllTableNameList();
            foreach (var tableName in tableNameList)
            {
                if (!DatabaseApi.Instance.IsTableExists(tableName))
                {
                    DatabaseApi.Instance.CreateTable(tableName, DataProvider.ContentRepository.TableColumns, string.Empty, true, out _, out _);
                }
                else
                {
                    DatabaseApi.Instance.AlterTable(tableName, DataProvider.ContentRepository.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        public static void UpdateConfigVersion()
        {
            var configInfo = DataProvider.Config.GetConfigInfo();
            if (configInfo == null)
            {
                configInfo = new ConfigInfo
                {
                    Initialized = true,
                    DatabaseVersion = Version,
                    UpdateDate = DateTime.Now
                };
                DataProvider.Config.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = Version;
                configInfo.Initialized = true;
                configInfo.UpdateDate = DateTime.Now;
                DataProvider.Config.Update(configInfo);
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
            var isNeedInstall = !DataProvider.Config.IsInitialized();
            if (isNeedInstall)
            {
                isNeedInstall = !DataProvider.Config.IsInitialized();
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
