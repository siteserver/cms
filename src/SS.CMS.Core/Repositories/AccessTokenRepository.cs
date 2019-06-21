using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(AccessTokenRepository));
        private readonly Repository<AccessTokenInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        public AccessTokenRepository(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _repository = new Repository<AccessTokenInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Title = nameof(AccessTokenInfo.Title);
        }

        public async Task<int> InsertAsync(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = _settingsManager.Encrypt(StringUtils.GetGuid());

            accessTokenInfo.Id = await _repository.InsertAsync(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                _cacheManager.Remove(CacheKey);
            }
            return accessTokenInfo.Id;
        }

        public async Task<bool> UpdateAsync(AccessTokenInfo accessTokenInfo)
        {
            var updated = await _repository.UpdateAsync(accessTokenInfo);
            if (updated)
            {
                _cacheManager.Remove(CacheKey);
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                _cacheManager.Remove(CacheKey);
            }
            return deleted;
        }

        public async Task<string> RegenerateAsync(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid(), _settingsManager.SecretKey);

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
                _cacheManager.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);

                    var retVal = new Dictionary<string, AccessTokenInfo>();
                    foreach (var accessTokenInfo in await _repository.GetAllAsync())
                    {
                        var token = TranslateUtils.DecryptStringBySecretKey(accessTokenInfo.Token, _settingsManager.SecretKey);
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
    }
}
