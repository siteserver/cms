using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class UserManager
    {
        private static class UserManagerCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(UserManager));

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
            }

            public static void Update(UserInfo userInfo)
            {
                if (userInfo == null) return;

                var dict = GetDict();

                lock (LockObject)
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

            public static void Remove(UserInfo userInfo)
            {
                if (userInfo == null) return;

                var dict = GetDict();

                lock (LockObject)
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

            private static string GetDictKeyByUserId(int userId)
            {
                return $"userId:{userId}";
            }

            private static string GetDictKeyByUserName(string userName)
            {
                return $"userName:{userName}";
            }

            private static string GetDictKeyByMobile(string mobile)
            {
                return $"mobile:{mobile}";
            }

            private static string GetDictKeyByEmail(string email)
            {
                return $"email:{email}";
            }

            public static UserInfo GetCacheByUserId(int userId)
            {
                if (userId <= 0) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserId(userId), out UserInfo userInfo);
                if (userInfo != null) return userInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByUserId(userId), out userInfo);

                    if (userInfo == null)
                    {
                        userInfo = DataProvider.UserDao.GetByUserId(userId);
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

            public static UserInfo GetCacheByUserName(string userName)
            {
                if (string.IsNullOrEmpty(userName)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserName(userName), out UserInfo userInfo);
                if (userInfo != null) return userInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByUserName(userName), out userInfo);

                    if (userInfo == null)
                    {
                        userInfo = DataProvider.UserDao.GetByUserName(userName);
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

            public static UserInfo GetCacheByMobile(string mobile)
            {
                if (string.IsNullOrEmpty(mobile)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByMobile(mobile), out UserInfo userInfo);
                if (userInfo != null) return userInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByMobile(mobile), out userInfo);

                    if (userInfo == null)
                    {
                        userInfo = DataProvider.UserDao.GetByMobile(mobile);
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

            public static UserInfo GetCacheByEmail(string email)
            {
                if (string.IsNullOrEmpty(email)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByEmail(email), out UserInfo userInfo);
                if (userInfo != null) return userInfo;

                lock (LockObject)
                {
                    dict.TryGetValue(GetDictKeyByEmail(email), out userInfo);

                    if (userInfo == null)
                    {
                        userInfo = DataProvider.UserDao.GetByEmail(email);
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

            private static Dictionary<string, UserInfo> GetDict()
            {
                var dict = DataCacheManager.Get<Dictionary<string, UserInfo>>(CacheKey);
                if (dict != null) return dict;

                lock (LockObject)
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
        }

        public static void ClearCache()
        {
            UserManagerCache.Clear();
        }

        public static void UpdateCache(UserInfo userInfo)
        {
            UserManagerCache.Update(userInfo);
        }

        public static void RemoveCache(UserInfo userInfo)
        {
            UserManagerCache.Remove(userInfo);
        }

        public static UserInfo GetUserInfoByUserId(int userId)
        {
            return UserManagerCache.GetCacheByUserId(userId);
        }

        public static UserInfo GetUserInfoByUserName(string userName)
        {
            return UserManagerCache.GetCacheByUserName(userName);
        }

        public static UserInfo GetUserInfoByMobile(string mobile)
        {
            return UserManagerCache.GetCacheByMobile(mobile);
        }

        public static UserInfo GetUserInfoByEmail(string email)
        {
            return UserManagerCache.GetCacheByEmail(email);
        }

        public static UserInfo GetUserInfoByAccount(string account)
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

        public static bool IsIpAddressCached(string ipAddress)
        {
            if (ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes == 0 || string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            var obj = CacheUtils.Get($"SiteServer.CMS.Provider.UserDao.Insert.IpAddress.{ipAddress}");
            return obj == null;
        }

        public static void CacheIpAddress(string ipAddress)
        {
            if (ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes > 0 && !string.IsNullOrEmpty(ipAddress))
            {
                CacheUtils.InsertMinutes($"SiteServer.CMS.Provider.UserDao.Insert.IpAddress.{ipAddress}", ipAddress, ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes);
            }
        }

        public static string GetHomeUploadPath(params string[] paths)
        {
            
            var path = PathUtils.GetSiteFilesPath(DirectoryUtils.SiteFiles.Home, PathUtils.Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public static string GetUserUploadPath(int userId, string relatedPath)
        {
            return GetHomeUploadPath(userId.ToString(), relatedPath);
        }

        public static string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        public static string GetHomeUploadUrl(params string[] paths)
        {
            return PageUtils.GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Home, PageUtils.Combine(paths)));
        }

        public static string DefaultAvatarUrl => GetHomeUploadUrl("default_avatar.png");

        public static string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetHomeUploadUrl(userId.ToString(), relatedUrl);
        }

        public static string GetUserAvatarUrl(IUserInfo userInfo)
        {
            var imageUrl = userInfo?.AvatarUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.IsProtocolUrl(imageUrl) ? imageUrl : GetUserUploadUrl(userInfo.Id, imageUrl);
            }

            return DefaultAvatarUrl;
        }
    }
}
