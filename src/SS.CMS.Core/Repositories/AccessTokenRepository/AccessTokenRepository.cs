using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly Repository<AccessToken> _repository;
        private readonly ISettingsManager _settingsManager;
        public AccessTokenRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(AreaRepository));
            _repository = new Repository<AccessToken>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Title = nameof(AccessToken.Title);
        }

        public async Task<int> InsertAsync(AccessToken accessTokenInfo)
        {
            accessTokenInfo.Token = _settingsManager.Encrypt(StringUtils.GetGuid());

            accessTokenInfo.Id = await _repository.InsertAsync(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                await _cache.RemoveAsync(_cacheKey);
            }
            return accessTokenInfo.Id;
        }

        public async Task<bool> UpdateAsync(AccessToken accessTokenInfo)
        {
            var updated = await _repository.UpdateAsync(accessTokenInfo);
            if (updated)
            {
                await _cache.RemoveAsync(_cacheKey);
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                await _cache.RemoveAsync(_cacheKey);
            }
            return deleted;
        }

        public async Task<string> RegenerateAsync(AccessToken accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid(), _settingsManager.SecurityKey);

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

        public async Task<AccessToken> GetAsync(string token)
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.TryGetValue(token, out var tokenInfo) ? tokenInfo : null;
        }

        public async Task<AccessToken> GetAsync(int id)
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.Where(x => x.Value.Id == id).Select(x => x.Value).FirstOrDefault();
        }

        public async Task<IEnumerable<AccessToken>> GetAllAsync()
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.Select(x => x.Value);
        }

        public async Task<bool> IsTitleExistsAsync(string title)
        {
            var dict = await GetAccessTokenDictionaryAsync();

            return dict.Any(x => x.Value != null && x.Value.Title == title);
        }

        private async Task<Dictionary<string, AccessToken>> GetAccessTokenDictionaryAsync()
        {
            return await
                _cache.GetOrCreateAsync(_cacheKey, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(1);

                    var retVal = new Dictionary<string, AccessToken>();
                    foreach (var accessTokenInfo in await _repository.GetAllAsync())
                    {
                        var token = TranslateUtils.DecryptStringBySecretKey(accessTokenInfo.Token, _settingsManager.SecurityKey);
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
