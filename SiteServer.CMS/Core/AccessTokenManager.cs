using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Collections.Generic;

namespace SiteServer.CMS.Core
{
	public static class AccessTokenManager
	{
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

        private static class AccessTokenManagerCache
        {
	        private static readonly object LockObject = new object();
	        private const string CacheKey = "SiteServer.CMS.Core.AccessTokenManager";

	        public static void Clear()
	        {
	            CacheUtils.Remove(CacheKey);
	        }

	        public static Dictionary<string, AccessTokenInfo> GetAccessTokenDictionary()
	        {
	            var retval = CacheUtils.Get<Dictionary<string, AccessTokenInfo>>(CacheKey);
	            if (retval != null) return retval;

	            lock (LockObject)
	            {
	                retval = CacheUtils.Get<Dictionary<string, AccessTokenInfo>>(CacheKey);
	                if (retval == null)
	                {
	                    retval = DataProvider.AccessTokenDao.GetAccessTokenInfoDictionary();

	                    CacheUtils.Insert(CacheKey, retval);
	                }
	            }

	            return retval;
	        }
	    }

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
