using System;
using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class UserRepository
    {
        private readonly object _lockObject = new object();

        private void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
        }

        private void UpdateCache(UserInfo userInfo)
        {
            if (userInfo == null) return;

            var dict = GetDict();

            lock (_lockObject)
            {
                dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                if (!string.IsNullOrEmpty(userInfo.Mobile))
                {
                    dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                }
                if (!string.IsNullOrEmpty(userInfo.Email))
                {
                    dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                }
            }
        }

        private void RemoveCache(UserInfo userInfo)
        {
            if (userInfo == null) return;

            var dict = GetDict();

            lock (_lockObject)
            {
                dict.Remove(GetDictKeyByUserId(userInfo.Id));
                dict.Remove(GetDictKeyByUserName(userInfo.UserName));
                if (!string.IsNullOrEmpty(userInfo.Mobile))
                {
                    dict.Remove(GetDictKeyByMobile(userInfo.Mobile));
                }

                if (!string.IsNullOrEmpty(userInfo.Email))
                {
                    dict.Remove(GetDictKeyByEmail(userInfo.Email));
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

        public UserInfo GetUserInfoByUserId(int userId)
        {
            if (userId <= 0) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserId(userId), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            lock (_lockObject)
            {
                dict.TryGetValue(GetDictKeyByUserId(userId), out userInfo);

                if (userInfo == null)
                {
                    userInfo = GetByUserId(userId);
                    if (userInfo != null)
                    {
                        dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                        dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                        if (!string.IsNullOrEmpty(userInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                        }
                        if (!string.IsNullOrEmpty(userInfo.Email))
                        {
                            dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                        }
                    }
                }
            }

            return userInfo;
        }

        public UserInfo GetUserInfoByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByUserName(userName), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            lock (_lockObject)
            {
                dict.TryGetValue(GetDictKeyByUserName(userName), out userInfo);

                if (userInfo == null)
                {
                    userInfo = GetByUserName(userName);
                    if (userInfo != null)
                    {
                        dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                        dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                        if (!string.IsNullOrEmpty(userInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                        }
                        if (!string.IsNullOrEmpty(userInfo.Email))
                        {
                            dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                        }
                    }
                }
            }

            return userInfo;
        }

        public UserInfo GetUserInfoByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByMobile(mobile), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            lock (_lockObject)
            {
                dict.TryGetValue(GetDictKeyByMobile(mobile), out userInfo);

                if (userInfo == null)
                {
                    userInfo = GetByMobile(mobile);
                    if (userInfo != null)
                    {
                        dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                        dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                        if (!string.IsNullOrEmpty(userInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                        }
                        if (!string.IsNullOrEmpty(userInfo.Email))
                        {
                            dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                        }
                    }
                }
            }

            return userInfo;
        }

        public UserInfo GetUserInfoByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var dict = GetDict();

            dict.TryGetValue(GetDictKeyByEmail(email), out UserInfo userInfo);
            if (userInfo != null) return userInfo;

            lock (_lockObject)
            {
                dict.TryGetValue(GetDictKeyByEmail(email), out userInfo);

                if (userInfo == null)
                {
                    userInfo = GetByEmail(email);
                    if (userInfo != null)
                    {
                        dict[GetDictKeyByUserId(userInfo.Id)] = userInfo;
                        dict[GetDictKeyByUserName(userInfo.UserName)] = userInfo;
                        if (!string.IsNullOrEmpty(userInfo.Mobile))
                        {
                            dict[GetDictKeyByMobile(userInfo.Mobile)] = userInfo;
                        }
                        if (!string.IsNullOrEmpty(userInfo.Email))
                        {
                            dict[GetDictKeyByEmail(userInfo.Email)] = userInfo;
                        }
                    }
                }
            }

            return userInfo;
        }

        private Dictionary<string, UserInfo> GetDict()
        {
            var dict = DataCacheManager.Get<Dictionary<string, UserInfo>>(CacheKey);
            if (dict != null) return dict;

            lock (_lockObject)
            {
                dict = DataCacheManager.Get<Dictionary<string, UserInfo>>(CacheKey);
                if (dict == null)
                {
                    dict = new Dictionary<string, UserInfo>();
                    DataCacheManager.Insert(CacheKey, dict);
                }
            }

            return dict;
        }

        public UserInfo GetUserInfoByAccount(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return GetUserInfoByMobile(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return GetUserInfoByEmail(account);
            }

            return GetUserInfoByUserName(account);
        }

        public bool IsIpAddressCached(string ipAddress)
        {
            if (_settingsManager.ConfigInfo.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            var obj = CacheUtils.Get($"SiteServer.CMS.Provider.User.InsertObject.IpAddress.{ipAddress}");
            return obj == null;
        }

        public void CacheIpAddress(string ipAddress)
        {
            if (_settingsManager.ConfigInfo.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                CacheUtils.InsertMinutes($"SiteServer.CMS.Provider.User.InsertObject.IpAddress.{ipAddress}", ipAddress, _settingsManager.ConfigInfo.UserRegistrationMinMinutes);
            }
        }
    }
}
