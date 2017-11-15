using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Data;

namespace SiteServer.CMS.Core
{
    public class SystemManager
    {
        public static void Install(string adminName, string adminPassword)
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

            BaiRongDataProvider.ConfigDao.InitializeConfig();
            BaiRongDataProvider.ConfigDao.InitializeUserRole(adminName, adminPassword);

            BaiRongDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();
        }

        public static void Upgrade()
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
            configInfo.DatabaseVersion = AppManager.Version;
            configInfo.IsInitialized = true;
            configInfo.UpdateDate = DateTime.Now;
            BaiRongDataProvider.ConfigDao.Update(configInfo);

            BaiRongDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();
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
