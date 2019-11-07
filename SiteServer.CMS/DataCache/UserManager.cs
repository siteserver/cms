using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
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

            public static void Update(User user)
            {
                if (user == null) return;

                var dict = GetDict();

                lock (LockObject)
                {
                    dict[GetDictKeyByUserId(user.Id)] = user;
                    dict[GetDictKeyByUserName(user.UserName)] = user;
                    if (!string.IsNullOrEmpty(user.Mobile))
                    {
                        dict[GetDictKeyByMobile(user.Mobile)] = user;
                    }
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        dict[GetDictKeyByEmail(user.Email)] = user;
                    }
                }
            }

            public static void Remove(User user)
            {
                if (user == null) return;

                var dict = GetDict();

                lock (LockObject)
                {
                    dict.Remove(GetDictKeyByUserId(user.Id));
                    dict.Remove(GetDictKeyByUserName(user.UserName));
                    if (!string.IsNullOrEmpty(user.Mobile))
                    {
                        dict.Remove(GetDictKeyByMobile(user.Mobile));
                    }

                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        dict.Remove(GetDictKeyByEmail(user.Email));
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

            public static async Task<User> GetCacheByUserIdAsync(int userId)
            {
                if (userId <= 0) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserId(userId), out User user);
                if (user != null) return user;

                dict.TryGetValue(GetDictKeyByUserId(userId), out user);

                if (user == null)
                {
                    user = await DataProvider.UserDao.GetByUserIdAsync(userId);
                    if (user != null)
                    {
                        dict[GetDictKeyByUserId(user.Id)] = user;
                        dict[GetDictKeyByUserName(user.UserName)] = user;
                        if (!string.IsNullOrEmpty(user.Mobile))
                        {
                            dict[GetDictKeyByMobile(user.Mobile)] = user;
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            dict[GetDictKeyByEmail(user.Email)] = user;
                        }
                    }
                }

                return user;
            }

            public static async Task<User> GetCacheByUserNameAsync(string userName)
            {
                if (string.IsNullOrEmpty(userName)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByUserName(userName), out User user);
                if (user != null) return user;

                dict.TryGetValue(GetDictKeyByUserName(userName), out user);

                if (user == null)
                {
                    user = await DataProvider.UserDao.GetByUserNameAsync(userName);
                    if (user != null)
                    {
                        dict[GetDictKeyByUserId(user.Id)] = user;
                        dict[GetDictKeyByUserName(user.UserName)] = user;
                        if (!string.IsNullOrEmpty(user.Mobile))
                        {
                            dict[GetDictKeyByMobile(user.Mobile)] = user;
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            dict[GetDictKeyByEmail(user.Email)] = user;
                        }
                    }
                }

                return user;
            }

            public static async Task<User> GetCacheByMobileAsync(string mobile)
            {
                if (string.IsNullOrEmpty(mobile)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByMobile(mobile), out User user);
                if (user != null) return user;

                dict.TryGetValue(GetDictKeyByMobile(mobile), out user);

                if (user == null)
                {
                    user = await DataProvider.UserDao.GetByMobileAsync(mobile);
                    if (user != null)
                    {
                        dict[GetDictKeyByUserId(user.Id)] = user;
                        dict[GetDictKeyByUserName(user.UserName)] = user;
                        if (!string.IsNullOrEmpty(user.Mobile))
                        {
                            dict[GetDictKeyByMobile(user.Mobile)] = user;
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            dict[GetDictKeyByEmail(user.Email)] = user;
                        }
                    }
                }

                return user;
            }

            public static async Task<User> GetCacheByEmailAsync(string email)
            {
                if (string.IsNullOrEmpty(email)) return null;

                var dict = GetDict();

                dict.TryGetValue(GetDictKeyByEmail(email), out User user);
                if (user != null) return user;

                dict.TryGetValue(GetDictKeyByEmail(email), out user);

                if (user == null)
                {
                    user = await DataProvider.UserDao.GetByEmailAsync(email);
                    if (user != null)
                    {
                        dict[GetDictKeyByUserId(user.Id)] = user;
                        dict[GetDictKeyByUserName(user.UserName)] = user;
                        if (!string.IsNullOrEmpty(user.Mobile))
                        {
                            dict[GetDictKeyByMobile(user.Mobile)] = user;
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            dict[GetDictKeyByEmail(user.Email)] = user;
                        }
                    }
                }

                return user;
            }

            private static Dictionary<string, User> GetDict()
            {
                var dict = DataCacheManager.Get<Dictionary<string, User>>(CacheKey);
                if (dict != null) return dict;

                lock (LockObject)
                {
                    dict = DataCacheManager.Get<Dictionary<string, User>>(CacheKey);
                    if (dict == null)
                    {
                        dict = new Dictionary<string, User>();
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

        public static void UpdateCache(User user)
        {
            UserManagerCache.Update(user);
        }

        public static void RemoveCache(User user)
        {
            UserManagerCache.Remove(user);
        }

        public static async Task<User> GetUserByUserIdAsync(int userId)
        {
            return await UserManagerCache.GetCacheByUserIdAsync(userId);
        }

        public static async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await UserManagerCache.GetCacheByUserNameAsync(userName);
        }

        public static async Task<User> GetUserByMobileAsync(string mobile)
        {
            return await UserManagerCache.GetCacheByMobileAsync(mobile);
        }

        public static async Task<User> GetUserByEmailAsync(string email)
        {
            return await UserManagerCache.GetCacheByEmailAsync(email);
        }

        public static async Task<User> GetUserByAccountAsync(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;

            if (StringUtils.IsMobile(account))
            {
                return await GetUserByMobileAsync(account);
            }
            if (StringUtils.IsEmail(account))
            {
                return await GetUserByEmailAsync(account);
            }

            return await GetUserByUserNameAsync(account);
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

        public static string GetUserAvatarUrl(User user)
        {
            var imageUrl = user?.AvatarUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.IsProtocolUrl(imageUrl) ? imageUrl : GetUserUploadUrl(user.Id, imageUrl);
            }

            return DefaultAvatarUrl;
        }
    }
}
