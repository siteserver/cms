using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Core.Common;
using SS.CMS.Core.Components;
using SS.CMS.Core.Models;
using SS.CMS.Core.Settings;
using SS.CMS.Data;
using SS.CMS.Utils;
using SS.CMS.Utils.Auth;

namespace SS.CMS.Core.Repositories
{
    

    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly string _cacheKey;
        private readonly Repository<AccessTokenInfo> _repository;
        private readonly AppSettings _settings;
        private readonly IMemoryCache _memoryCache;
        public AccessTokenRepository(IDb db, AppSettings settings, IMemoryCache memoryCache)
        {
            _cacheKey = StringUtils.GetCacheKey(nameof(AccessTokenRepository));
            _repository = new Repository<AccessTokenInfo>(db);
            _settings = settings;
            _memoryCache = memoryCache;
        }

        public IDb Db => _repository.Db;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Title = nameof(AccessTokenInfo.Title);
        }

        public async Task<int> InsertAsync(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = Settings.AppContext.Encrypt(StringUtils.GetGuid());
            accessTokenInfo.AddDate = DateTime.Now;

            accessTokenInfo.Id = await _repository.InsertAsync(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                _memoryCache.Remove(_cacheKey);
            }
            return accessTokenInfo.Id;
        }

        public async Task<bool> UpdateAsync(AccessTokenInfo accessTokenInfo)
        {
            var updated = await _repository.UpdateAsync(accessTokenInfo);
            if (updated)
            {
                _memoryCache.Remove(_cacheKey);
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                _memoryCache.Remove(_cacheKey);
            }
            return deleted;
        }

        public async Task<string> RegenerateAsync(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid(), _settings.SecretKey);

            await UpdateAsync(accessTokenInfo);

            return accessTokenInfo.Token;
        }

        public async Task<bool> IsScopeAsync(string token, string scope)
        {
            if (string.IsNullOrEmpty(token)) return false;

            var tokenInfo = await GetAsync(token);
            if (tokenInfo == null) return false;

            return StringUtils.ContainsIgnoreCase(TranslateUtils.StringCollectionToStringList(tokenInfo.Scopes), scope);
        }

        public async Task<AccessTokenInfo> GetAsync(string token)
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.TryGetValue(token, out var tokenInfo) ? tokenInfo : null;
        }

        public async Task<AccessTokenInfo> GetAsync(int id)
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.Where(x => x.Value.Id == id).Select(x => x.Value).FirstOrDefault();
        }

        public async Task<IEnumerable<AccessTokenInfo>> GetAllAsync()
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.Select(x => x.Value);
        }

        public async Task<bool> IsTitleExistsAsync(string title)
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.Any(x => x.Value != null && x.Value.Title == title);
        }

        private async Task<Dictionary<string, AccessTokenInfo>> GetAccessTokenDictionaryAsync()
        {
            return await
                _memoryCache.GetOrCreateAsync(_cacheKey, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);

                    var retVal = new Dictionary<string, AccessTokenInfo>();
                    foreach (var accessTokenInfo in await _repository.GetAllAsync())
                    {
                        var token = TranslateUtils.DecryptStringBySecretKey(accessTokenInfo.Token, _settings.SecretKey);
                        if (!string.IsNullOrEmpty(token))
                        {
                            retVal[token] = accessTokenInfo;
                        }
                    }

                    return retVal;
                });
        }

        public string ScopeContents => "Contents";
        public string ScopeAdministrators => "Administrators";
        public string ScopeUsers => "Users";
        public string ScopeStl => "STL";

        public List<string> ScopeList => new List<string>
        {
            ScopeContents,
            ScopeAdministrators,
            ScopeUsers,
            ScopeStl
        };

        public string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = DateTime.Now.Add(expiresAt)
            };

            return JsonWebToken.Encode(userToken, _settings.SecretKey, JwtHashAlgorithm.HS256);
        }

        public string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = expiresAt
            };

            return JsonWebToken.Encode(userToken, _settings.SecretKey, JwtHashAlgorithm.HS256);
        }

        public IAccessToken ParseAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return new AccessTokenImpl();

            try
            {
                var tokenObj = JsonWebToken.DecodeToObject<AccessTokenImpl>(accessToken, _settings.SecretKey);

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
