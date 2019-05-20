using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Auth;

namespace SiteServer.CMS.DataCache
{
    public static class AccessTokenManager
    {
        private static class AccessTokenManagerCache
        {
            private static readonly object LockObject = new object();

            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(AccessTokenManager));

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
            }

            public static Dictionary<string, AccessTokenInfo> GetAccessTokenDictionary()
            {
                var retVal = DataCacheManager.Get<Dictionary<string, AccessTokenInfo>>(CacheKey);
                if (retVal != null) return retVal;

                lock (LockObject)
                {
                    retVal = DataCacheManager.Get<Dictionary<string, AccessTokenInfo>>(CacheKey);
                    if (retVal != null) return retVal;

                    retVal = new Dictionary<string, AccessTokenInfo>();
                    foreach (var accessTokenInfo in DataProvider.AccessTokenDao.GetAll())
                    {
                        var token = TranslateUtils.DecryptStringBySecretKey(accessTokenInfo.Token);
                        if (!string.IsNullOrEmpty(token))
                        {
                            retVal[token] = accessTokenInfo;
                        }
                    }

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
            }
        }

        public const string ScopeContents = "Contents";
        public const string ScopeAdministrators = "Administrators";
        public const string ScopeUsers = "Users";
        public const string ScopeStl = "STL";

        public static List<string> ScopeList => new List<string>
        {
            ScopeContents,
            ScopeAdministrators,
            ScopeUsers,
            ScopeStl
        };

        public static void ClearCache()
        {
            AccessTokenManagerCache.Clear();
        }

        public static bool IsScope(string token, string scope)
        {
            if (string.IsNullOrEmpty(token)) return false;

            var tokenInfo = GetAccessTokenInfo(token);
            if (tokenInfo == null) return false;

            return StringUtils.ContainsIgnoreCase(TranslateUtils.StringCollectionToStringList(tokenInfo.Scopes), scope);
        }

        public static AccessTokenInfo GetAccessTokenInfo(string token)
        {
            AccessTokenInfo tokenInfo = null;
            var dict = AccessTokenManagerCache.GetAccessTokenDictionary();

            if (dict != null && dict.ContainsKey(token))
            {
                tokenInfo = dict[token];
            }
            return tokenInfo;
        }

        public static string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = DateTime.Now.Add(expiresAt)
            };

            return JsonWebToken.Encode(userToken, AppSettings.SecretKey, JwtHashAlgorithm.HS256);
        }

        public static string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = expiresAt
            };

            return JsonWebToken.Encode(userToken, AppSettings.SecretKey, JwtHashAlgorithm.HS256);
        }

        public static AccessTokenImpl ParseAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return new AccessTokenImpl();

            try
            {
                var tokenObj = JsonWebToken.DecodeToObject<AccessTokenImpl>(accessToken, AppSettings.SecretKey);

                if (tokenObj?.ExpiresAt.AddDays(Constants.AccessTokenExpireDays) > DateTime.Now)
                {
                    return tokenObj;
                }
            }
            catch
            {
                // ignored
            }

            return new AccessTokenImpl();
        }
    }
}
