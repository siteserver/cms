using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Common
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

        public static void InstallDatabase(string adminName, string adminPassword, IAdministratorRepository administratorRepository, IAdministratorsInRolesRepository administratorsInRolesRepository, IConfigRepository configRepository)
        {
            SyncDatabase(configRepository);

            if (!string.IsNullOrEmpty(adminName) && !string.IsNullOrEmpty(adminPassword))
            {
                var administratorInfo = new AdministratorInfo
                {
                    UserName = adminName,
                    Password = adminPassword
                };

                administratorRepository.Insert(administratorInfo, out _);
                administratorsInRolesRepository.AddUserToRole(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
            }
        }

        public static void SyncSystemTables()
        {
            //foreach (var repository in DataProvider.AllProviders)
            //{
            //    if (string.IsNullOrEmpty(repository.TableName) || repository.TableColumns == null || repository.TableColumns.Count <= 0) continue;

            //    if (!AppContext.Db.IsTableExists(repository.TableName))
            //    {
            //        TableColumnManager.CreateTable(repository.TableName, repository.TableColumns, string.Empty, false, out _);
            //    }
            //    else
            //    {
            //        TableColumnManager.AlterTable(repository.TableName, repository.TableColumns, string.Empty);
            //    }
            //}
        }

        public static void SyncContentTables()
        {
            // var contentDaoList = ContentRepository.GetContentDaoList();
            // foreach (var contentDao in contentDaoList)
            // {
            //     if (!AppContext.Db.IsTableExists(contentDao.TableName))
            //     {
            //         TableColumnManager.CreateTable(contentDao.TableName, contentDao.TableColumns, string.Empty, true, out _);
            //     }
            //     else
            //     {
            //         TableColumnManager.AlterTable(contentDao.TableName, contentDao.TableColumns, string.Empty, ContentAttribute.DropAttributes.Value);
            //     }
            // }
        }

        public static void UpdateConfigVersion(IConfigRepository configRepository)
        {
            var configInfo = configRepository.GetConfigInfo();
            if (configInfo == null)
            {
                configInfo = new ConfigInfo
                {
                    Initialized = true,
                    DatabaseVersion = ProductVersion,
                    UpdateDate = DateTime.Now
                };
                configRepository.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = ProductVersion;
                configInfo.Initialized = true;
                configInfo.UpdateDate = DateTime.Now;
                configRepository.Update(configInfo);
            }
        }

        public static void SyncDatabase(IConfigRepository configRepository)
        {
            CacheUtils.ClearAll();

            SyncSystemTables();

            SyncContentTables();

            UpdateConfigVersion(configRepository);
        }

        public static bool IsNeedInstall(IConfigRepository configRepository)
        {
            var isNeedInstall = !configRepository.IsInitialized();
            if (isNeedInstall)
            {
                isNeedInstall = !configRepository.IsInitialized();
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
