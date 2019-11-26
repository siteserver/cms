using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Provider;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class SystemManager
    {
        public static async Task LoadSettingsAsync(string applicationPhysicalPath)
        {
            WebConfigUtils.Load(applicationPhysicalPath, PathUtils.Combine(applicationPhysicalPath, WebConfigUtils.WebConfigFileName));

            await CacheManager.LoadCacheAsync();

            await PluginManager.LoadPluginsAsync(applicationPhysicalPath);

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
        }

        public static string ProductVersion { get; private set; }

        public static string PluginVersion { get; private set; }

        public static string TargetFramework { get; private set; }

        public static string EnvironmentVersion { get; private set; }

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

        public static async Task CreateSiteServerTablesAsync()
        {
            foreach (var provider in DataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!DataProvider.DatabaseDao.IsTableExists(provider.TableName))
                {
                    await DataProvider.DatabaseDao.CreateTableAsync(provider.TableName, provider.TableColumns);
                }
                else
                {
                    await DataProvider.DatabaseDao.AlterSystemTableAsync(provider.TableName, provider.TableColumns);
                }
            }
        }

        public static async Task SyncContentTablesAsync()
        {
            var tableNameList = await DataProvider.SiteDao.GetAllTableNameListAsync();
            foreach (var tableName in tableNameList)
            {
                if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    await DataProvider.DatabaseDao.CreateTableAsync(tableName, DataProvider.ContentDao.TableColumns);
                }
                else
                {
                    await DataProvider.DatabaseDao.AlterSystemTableAsync(tableName, DataProvider.ContentDao.TableColumns, ContentAttribute.DropAttributes.Value);
                }
            }
        }

        public static async Task UpdateConfigVersionAsync()
        {
            var config = await DataProvider.ConfigDao.GetAsync();
            if (config.Id == 0)
            {
                config = new Config
                {
                    Id = 0,
                    DatabaseVersion = ProductVersion,
                    UpdateDate = DateTime.Now
                };
                config.Id = await DataProvider.ConfigDao.InsertAsync(config);
            }
            else
            {
                config.DatabaseVersion = ProductVersion;
                config.UpdateDate = DateTime.Now;
                await DataProvider.ConfigDao.UpdateAsync(config);
            }
        }

        public static async Task SyncDatabaseAsync()
        {
            //CacheUtils.ClearAll();

            //await CreateSiteServerTablesAsync();

            //await SyncContentTablesAsync();

            //await UpdateConfigVersionAsync();

            await RepositoryManager.SyncDatabaseAsync();
        }

        public static async Task<bool> IsNeedInstallAsync()
        {
            var isNeedInstall = !await DataProvider.ConfigDao.IsInitializedAsync();
            if (isNeedInstall)
            {
                isNeedInstall = !await DataProvider.ConfigDao.IsInitializedAsync();
            }
            return isNeedInstall;
        }
    }
}
