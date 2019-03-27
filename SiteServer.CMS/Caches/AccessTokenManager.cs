using System.Collections.Generic;
using SiteServer.CMS.Caches.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Caches
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
	                foreach (var accessTokenInfo in DataProvider.AccessToken.GetAll())
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
	        return tokenInfo != null &&
	               StringUtils.ContainsIgnoreCase(TranslateUtils.StringCollectionToStringList(tokenInfo.Scopes), scope);
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
    }
}
