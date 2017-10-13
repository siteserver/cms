using System;
using BaiRong.Core;

namespace SiteServer.CMS.Core
{
    public class UpgradeManager
    {
        public static void Upgrade()
        {
            foreach (var provider in BaiRongDataProvider.AllProviders)
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
        }

        //public static void Upgrade(string version, out string errorMessage)
        //{
        //    errorMessage = string.Empty;
        //    if (!string.IsNullOrEmpty(version) && BaiRongDataProvider.ConfigDao.GetDatabaseVersion() != version)
        //    {
        //        var errorBuilder = new StringBuilder();
        //        BaiRongDataProvider.DatabaseDao.Upgrade(WebConfigUtils.DatabaseType, errorBuilder);

        //        //升级数据库

        //        errorMessage = $"<!--{errorBuilder}-->";
        //    }

        //    var configInfo = BaiRongDataProvider.ConfigDao.GetConfigInfo();
        //    configInfo.DatabaseVersion = version;
        //    configInfo.IsInitialized = true;
        //    configInfo.UpdateDate = DateTime.Now;
        //    BaiRongDataProvider.ConfigDao.Update(configInfo);
        //}
    }
}
