using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
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

        public static async Task InstallDatabaseAsync(string adminName, string adminPassword)
        {
            await SyncDatabaseAsync();

            if (!string.IsNullOrEmpty(adminName) && !string.IsNullOrEmpty(adminPassword))
            {
                var administrator = new Administrator
                {
                    UserName = adminName,
                };

                await DataProvider.AdministratorDao.InsertAsync(administrator, adminPassword);
                await DataProvider.AdministratorsInRolesDao.AddUserToRoleAsync(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
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

        public static async Task SyncContentTablesAsync()
        {
            var tableNameList = await SiteManager.GetAllTableNameListAsync();
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
                configInfo = new ConfigInfo(0, true, ProductVersion, DateTime.Now, string.Empty);
                DataProvider.ConfigDao.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = ProductVersion;
                configInfo.IsInitialized = true;
                configInfo.UpdateDate = DateTime.Now;
                DataProvider.ConfigDao.Update(configInfo);
            }
        }

        public static async Task SyncDatabaseAsync()
        {
            CacheUtils.ClearAll();

            CreateSiteServerTables();

            await SyncContentTablesAsync();

            UpdateConfigVersion();
        }


        public static bool IsNeedUpdate()
        {
            return !StringUtils.EqualsIgnoreCase(ProductVersion, DataProvider.ConfigDao.GetDatabaseVersion());
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
