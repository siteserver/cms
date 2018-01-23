using System;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public class SystemManager
    {
        public static void Install(string adminName, string adminPassword)
        {
            InstallOrUpdate(adminName, adminPassword);
        }

        public static void Update()
        {
            InstallOrUpdate(string.Empty, string.Empty);
        }

        private static void InstallOrUpdate(string adminName, string adminPassword)
        {
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
                configInfo = new ConfigInfo(true, AppManager.Version, DateTime.Now, string.Empty);
                DataProvider.ConfigDao.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = AppManager.Version;
                configInfo.IsInitialized = true;
                configInfo.UpdateDate = DateTime.Now;
                DataProvider.ConfigDao.Update(configInfo);
            }

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

            DataProvider.TableDao.CreateAllTableCollectionInfoIfNotExists();
        }

        public static bool IsNeedUpdate()
        {
            return !StringUtils.EqualsIgnoreCase(AppManager.Version, DataProvider.ConfigDao.GetDatabaseVersion());
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
