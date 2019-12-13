using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using SiteServer.CMS.Caching;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;

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
                PluginVersion = FileVersionInfo.GetVersionInfo(PathUtils.GetBinDirectoryPath("SiteServer.Abstractions.dll")).ProductVersion;

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

                await DataProvider.AdministratorRepository.InsertAsync(administrator, adminPassword);
                await DataProvider.AdministratorsInRolesRepository.AddUserToRoleAsync(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
            }
        }

        public static async Task CreateSiteServerTablesAsync()
        {
            foreach (var provider in DataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!await WebConfigUtils.Database.IsTableExistsAsync(provider.TableName))
                {
                    await WebConfigUtils.Database.CreateTableAsync(provider.TableName, provider.TableColumns);
                }
                else
                {
                    await WebConfigUtils.Database.AlterTableAsync(provider.TableName, provider.TableColumns);
                }
            }
        }

        public static async Task SyncContentTablesAsync()
        {
            var tableNameList = await DataProvider.SiteRepository.GetAllTableNameListAsync();
            foreach (var tableName in tableNameList)
            {
                if (!await WebConfigUtils.Database.IsTableExistsAsync(tableName))
                {
                    await WebConfigUtils.Database.CreateTableAsync(tableName, DataProvider.ContentRepository.GetTableColumns(tableName));
                }
                else
                {
                    await WebConfigUtils.Database.AlterTableAsync(tableName, DataProvider.ContentRepository.GetTableColumns(tableName), ContentAttribute.DropAttributes.Value);
                }
            }
        }

        public static async Task UpdateConfigVersionAsync()
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
            if (config.Id == 0)
            {
                config = new Config
                {
                    Id = 0,
                    DatabaseVersion = ProductVersion,
                    UpdateDate = DateTime.Now
                };
                config.Id = await DataProvider.ConfigRepository.InsertAsync(config);
            }
            else
            {
                config.DatabaseVersion = ProductVersion;
                config.UpdateDate = DateTime.Now;
                await DataProvider.ConfigRepository.UpdateAsync(config);
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
            var isNeedInstall = !await DataProvider.ConfigRepository.IsInitializedAsync();
            if (isNeedInstall)
            {
                isNeedInstall = !await DataProvider.ConfigRepository.IsInitializedAsync();
            }
            return isNeedInstall;
        }
    }
}
