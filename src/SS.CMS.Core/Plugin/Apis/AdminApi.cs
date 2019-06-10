using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Components;

namespace SS.CMS.Core.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        private static AdminApi _instance;
        public static AdminApi Instance => _instance ?? (_instance = new AdminApi());

        public IAdministratorInfo GetAdminInfoByUserId(int userId)
        {
            return AdminManager.GetAdminInfoByUserId(userId);
        }

        public IAdministratorInfo GetAdminInfoByUserName(string userName)
        {
            return AdminManager.GetAdminInfoByUserName(userName);
        }

        public IAdministratorInfo GetAdminInfoByEmail(string email)
        {
            return AdminManager.GetAdminInfoByEmail(email);
        }

        public IAdministratorInfo GetAdminInfoByMobile(string mobile)
        {
            return AdminManager.GetAdminInfoByMobile(mobile);
        }

        public IAdministratorInfo GetAdminInfoByAccount(string account)
        {
            return AdminManager.GetAdminInfoByAccount(account);
        }

        public IPermissions GetPermissions(string userName)
        {
            return new PermissionsImpl(AdminManager.GetAdminInfoByUserName(userName));
        }

        public bool IsUserNameExists(string userName)
        {
            return DataProvider.AdministratorDao.IsUserNameExists(userName);
        }

        public bool IsEmailExists(string email)
        {
            return DataProvider.AdministratorDao.IsEmailExists(email);
        }

        public bool IsMobileExists(string mobile)
        {
            return DataProvider.AdministratorDao.IsMobileExists(mobile);
        }

        public void Log(string userName, int siteId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, 0, 0, string.Empty, userName, action, summary);
        }

        public void Log(string userName, int siteId, int channelId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, channelId, 0, string.Empty, userName, action, summary);
        }

        public void Log(string userName, int siteId, int channelId, int contentId, string action, string summary = "")
        {
            LogUtils.AddSiteLog(siteId, channelId, contentId, string.Empty, userName, action, summary);
        }

        public void Log(string userName, string action, string summary = "")
        {
            LogUtils.AddAdminLog(string.Empty, userName, action, summary);
        }
    }
}
