using System;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Tests.Utils
{
    public static class AdminUtils
    {
        public static AdministratorInfo CreateAdminIfNotExists()
        {
            var adminInfo = AdminManager.GetAdminInfoByUserName(nameof(CreateAdminIfNotExists));
            if (adminInfo != null) return adminInfo;

            adminInfo = new AdministratorInfo
            {
                UserName = nameof(CreateAdminIfNotExists),
                Password = Guid.NewGuid().ToString()
            };
            DataProvider.Administrator.Insert(adminInfo, out _);

            DataProvider.AdministratorsInRoles.AddUserToRole(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator));

            return adminInfo;
        }

        public static AdministratorInfo CreateSuperAdminIfNotExists()
        {
            var adminInfo = AdminManager.GetAdminInfoByUserName(nameof(CreateSuperAdminIfNotExists));
            if (adminInfo != null) return adminInfo;

            adminInfo = new AdministratorInfo
            {
                UserName = nameof(CreateSuperAdminIfNotExists),
                Password = Guid.NewGuid().ToString()
            };
            DataProvider.Administrator.Insert(adminInfo, out _);

            DataProvider.AdministratorsInRoles.AddUserToRole(adminInfo.UserName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));

            return adminInfo;
        }

        public static string GetAccessToken(AdministratorInfo adminInfo)
        {
            var expiresAt = TimeSpan.FromDays(7);
            return Rest.GetAccessToken(adminInfo.Id, adminInfo.UserName, expiresAt);
        }
    }
}
