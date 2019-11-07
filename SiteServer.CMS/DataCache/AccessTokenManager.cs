using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
	public static class AccessTokenManager
	{
	    private static class AccessTokenManagerCache
	    {
	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(AccessTokenManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static async Task<Dictionary<string, AccessTokenInfo>> GetAccessTokenDictionaryAsync()
	        {
	            var retVal = DataCacheManager.Get<Dictionary<string, AccessTokenInfo>>(CacheKey);
	            if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<Dictionary<string, AccessTokenInfo>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.AccessTokenDao.GetAccessTokenInfoDictionaryAsync();

                    DataCacheManager.Insert(CacheKey, retVal);
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

	    public static async Task<bool> IsScopeAsync(string token, string scope)
	    {
	        if (string.IsNullOrEmpty(token)) return false;

	        var tokenInfo = await GetAccessTokenInfoAsync(token);
	        if (tokenInfo == null) return false;

	        return StringUtils.ContainsIgnoreCase(TranslateUtils.StringCollectionToStringList(tokenInfo.Scopes), scope);
	    }

        public static async Task<AccessTokenInfo> GetAccessTokenInfoAsync(string token)
        {
            AccessTokenInfo tokenInfo = null;
            var dict = await AccessTokenManagerCache.GetAccessTokenDictionaryAsync();

            if (dict != null && dict.ContainsKey(token))
            {
                tokenInfo = dict[token];
            }
            return tokenInfo;
        }
    }
}
