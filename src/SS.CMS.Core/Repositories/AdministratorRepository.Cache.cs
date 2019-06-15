using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Repositories
{
    public partial class AdministratorRepository
    {
        private readonly object LockObject = new object();

        public void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
        }

        public void UpdateCache(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return;

            var dict = GetDict();

            lock (LockObject)
            {
                dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                if (!string.IsNullOrEmpty(adminInfo.Mobile))
                {
                    dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                }
                if (!string.IsNullOrEmpty(adminInfo.Email))
                {
                    dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                }
            }
        }

        public void RemoveCache(AdministratorInfo adminInfo)
        {
            if (adminInfo == null) return;

            var dict = GetDict();

            lock (LockObject)
            {
                dict.Remove(GetDictKeyByUserId(adminInfo.Id));
                dict.Remove(GetDictKeyByUserName(adminInfo.UserName));
                if (!string.IsNullOrEmpty(adminInfo.Mobile))
                {
                    dict.Remove(GetDictKeyByMobile(adminInfo.Mobile));
                }

                if (!string.IsNullOrEmpty(adminInfo.Email))
                {
                    dict.Remove(GetDictKeyByEmail(adminInfo.Email));
                }
            }
        }

        private string GetDictKeyByUserId(int userId)
        {
            return $"userId:{userId}";
        }

        private string GetDictKeyByUserName(string userName)
        {
            return $"userName:{userName}";
        }

        private string GetDictKeyByMobile(string mobile)
        {
            return $"mobile:{mobile}";
        }

        private string GetDictKeyByEmail(string email)
        {
            return $"email:{email}";
        }

        public AdministratorInfo GetAdminInfoByUserId(int userId)
        {
            if (userId <= 0) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserId(userId), out AdministratorInfo adminInfo);
            if (adminInfo != null) return adminInfo;

            lock (LockObject)
            {
                dict.TryGetValue(GetDictKeyByUserId(userId), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = GetByUserId(userId);
                    if (adminInfo != null)
                    {
                        dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                        dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                        if (!string.IsNullOrEmpty(adminInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                        }
                        if (!string.IsNullOrEmpty(adminInfo.Email))
                        {
                            dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                        }
                    }
                }
            }

            return adminInfo;
        }

        public AdministratorInfo GetAdminInfoByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserName(userName), out AdministratorInfo adminInfo);
            if (adminInfo != null) return adminInfo;

            lock (LockObject)
            {
                dict.TryGetValue(GetDictKeyByUserName(userName), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = GetByUserName(userName);
                    if (adminInfo != null)
                    {
                        dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                        dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                        if (!string.IsNullOrEmpty(adminInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                        }
                        if (!string.IsNullOrEmpty(adminInfo.Email))
                        {
                            dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                        }
                    }
                }
            }

            return adminInfo;
        }

        public AdministratorInfo GetAdminInfoByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByMobile(mobile), out AdministratorInfo adminInfo);
            if (adminInfo != null) return adminInfo;

            lock (LockObject)
            {
                dict.TryGetValue(GetDictKeyByMobile(mobile), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = GetByMobile(mobile);
                    if (adminInfo != null)
                    {
                        dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                        dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                        if (!string.IsNullOrEmpty(adminInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                        }
                        if (!string.IsNullOrEmpty(adminInfo.Email))
                        {
                            dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                        }
                    }
                }
            }

            return adminInfo;
        }

        public AdministratorInfo GetAdminInfoByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByEmail(email), out AdministratorInfo adminInfo);
            if (adminInfo != null) return adminInfo;

            lock (LockObject)
            {
                dict.TryGetValue(GetDictKeyByEmail(email), out adminInfo);

                if (adminInfo == null)
                {
                    adminInfo = GetByEmail(email);
                    if (adminInfo != null)
                    {
                        dict[GetDictKeyByUserId(adminInfo.Id)] = adminInfo;
                        dict[GetDictKeyByUserName(adminInfo.UserName)] = adminInfo;
                        if (!string.IsNullOrEmpty(adminInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(adminInfo.Mobile)] = adminInfo;
                        }
                        if (!string.IsNullOrEmpty(adminInfo.Email))
                        {
                            dict[GetDictKeyByEmail(adminInfo.Email)] = adminInfo;
                        }
                    }
                }
            }

            return adminInfo;
        }

        private Dictionary<string, AdministratorInfo> GetDict()
        {
            var dict = DataCacheManager.Get<Dictionary<string, AdministratorInfo>>(CacheKey);
            if (dict != null) return dict;

            lock (LockObject)
            {
                dict = DataCacheManager.Get<Dictionary<string, AdministratorInfo>>(CacheKey);
                if (dict == null)
                {
                    dict = new Dictionary<string, AdministratorInfo>();
                    DataCacheManager.Insert(CacheKey, dict);
                }
            }

            return dict;
        }

        public AdministratorInfo GetAdminInfoByAccount(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return GetAdminInfoByMobile(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return GetAdminInfoByEmail(account);
            }

            return GetAdminInfoByUserName(account);
        }

        public List<int> GetLatestTop10SiteIdList(List<int> siteIdListLatestAccessed, List<int> siteIdListOrderByLevel, List<int> siteIdListWithPermissions)
        {
            var siteIdList = new List<int>();

            foreach (var siteId in siteIdListLatestAccessed)
            {
                if (siteIdList.Count >= 10) break;
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            if (siteIdList.Count < 10)
            {
                // var siteIdListOrderByLevel = SiteManager.GetSiteIdListOrderByLevel();
                foreach (var siteId in siteIdListOrderByLevel)
                {
                    if (siteIdList.Count >= 5) break;
                    if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                    {
                        siteIdList.Add(siteId);
                    }
                }
            }

            return siteIdList;
        }

        public string GetDisplayName(string userName)
        {
            var adminInfo = GetAdminInfoByUserName(userName);
            if (adminInfo == null) return userName;

            return adminInfo.DisplayName;
        }

        public string GetRoleNames(string userName)
        {
            var isConsoleAdministrator = false;
            var isSystemAdministrator = false;
            var roleNameList = new List<string>();
            var roles = _administratorsInRolesRepository.GetRolesForUser(userName);
            foreach (var role in roles)
            {
                if (!EPredefinedRoleUtils.IsPredefinedRole(role))
                {
                    roleNameList.Add(role);
                }
                else
                {
                    if (EPredefinedRoleUtils.Equals(EPredefinedRole.ConsoleAdministrator, role))
                    {
                        isConsoleAdministrator = true;
                        break;
                    }
                    if (EPredefinedRoleUtils.Equals(EPredefinedRole.SystemAdministrator, role))
                    {
                        isSystemAdministrator = true;
                        break;
                    }
                }
            }

            var roleNames = string.Empty;

            if (isConsoleAdministrator)
            {
                roleNames += EPredefinedRoleUtils.GetText(EPredefinedRole.ConsoleAdministrator);
            }
            else if (isSystemAdministrator)
            {
                roleNames += EPredefinedRoleUtils.GetText(EPredefinedRole.SystemAdministrator);
            }
            else
            {
                roleNames += TranslateUtils.ObjectCollectionToString(roleNameList);
            }
            return roleNames;
        }

        public bool IsSuperAdmin(string userName)
        {
            var roles = _administratorsInRolesRepository.GetRolesForUser(userName);
            return roles.Any(role => EPredefinedRoleUtils.Equals(EPredefinedRole.ConsoleAdministrator, role));
        }
    }
}