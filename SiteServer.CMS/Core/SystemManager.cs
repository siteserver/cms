using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public class SystemManager
    {
        public static void Install(string adminName, string adminPassword)
        {
            InstallOrUpgrade(adminName, adminPassword);
        }

        public static void Upgrade()
        {
            InstallOrUpgrade(string.Empty, string.Empty);
        }

        private static void InstallOrUpgrade(string adminName, string adminPassword)
        {
            var providers = new List<DataProviderBase>();
            providers.AddRange(BaiRongDataProvider.AllProviders);
            providers.AddRange(DataProvider.AllProviders);

            foreach (var provider in providers)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null || provider.TableColumns.Count <= 0) continue;

                if (!BaiRongDataProvider.DatabaseDao.IsTableExists(provider.TableName))
                {
                    BaiRongDataProvider.DatabaseDao.CreateSystemTable(provider.TableName, provider.TableColumns);
                }
                else
                {
                    BaiRongDataProvider.DatabaseDao.AlterSystemTable(provider.TableName, provider.TableColumns);
                }
            }

            var configInfo = BaiRongDataProvider.ConfigDao.GetConfigInfo();
            if (configInfo == null)
            {
                configInfo = new ConfigInfo(true, AppManager.Version, DateTime.Now, string.Empty);
                BaiRongDataProvider.ConfigDao.Insert(configInfo);
            }
            else
            {
                configInfo.DatabaseVersion = AppManager.Version;
                configInfo.IsInitialized = true;
                configInfo.UpdateDate = DateTime.Now;
                BaiRongDataProvider.ConfigDao.Update(configInfo);
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
                BaiRongDataProvider.AdministratorsInRolesDao.AddUserToRole(adminName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
            }

            BaiRongDataProvider.TableCollectionDao.CreateAllTableCollectionInfoIfNotExists();
        }

        public static bool IsNeedUpgrade()
        {
            return !StringUtils.EqualsIgnoreCase(AppManager.Version, BaiRongDataProvider.ConfigDao.GetDatabaseVersion());
        }

        public static bool IsNeedInstall()
        {
            var isNeedInstall = !BaiRongDataProvider.ConfigDao.IsInitialized();
            if (isNeedInstall)
            {
                isNeedInstall = !BaiRongDataProvider.ConfigDao.IsInitialized();
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
