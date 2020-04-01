using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

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
	                if (retVal == null)
	                {
	                    retVal = DataProvider.AccessTokenDao.GetAccessTokenInfoDictionary();

	                    DataCacheManager.Insert(CacheKey, retVal);
	                }
	            }

	            return retVal;
	        }
	    }

        public const string ScopeChannels = "Channels";
        public const string ScopeContents = "Contents";
        public const string ScopeAdministrators = "Administrators";
	    public const string ScopeUsers = "Users";
        public const string ScopeStl = "STL";

	    public static List<string> ScopeList => new List<string>
	    {
            ScopeChannels,
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
    }
}
