using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.Caching.Distributed;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class AccessTokenRepository : IRepository
    {
        private readonly Repository<AccessToken> _repository;
        private readonly IDistributedCache _cache;

        public AccessTokenRepository()
        {
            _repository = new Repository<AccessToken>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
            _cache = CacheManager.Cache;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(AccessToken accessToken)
        {
            var token = WebConfigUtils.EncryptStringBySecretKey(StringUtils.Guid());
            accessToken.Token = token;
            accessToken.AddDate = DateTime.Now;
            accessToken.UpdatedDate = DateTime.Now;

            return await _repository.InsertAsync(accessToken);
        }

        public async Task<bool> UpdateAsync(AccessToken accessToken)
        {
            var updated = await _repository.UpdateAsync(accessToken);
            if (updated)
            {
                await RemoveCacheAsync(accessToken.Token);
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var accessToken = await _repository.GetAsync(id);
            if (accessToken == null) return false;

            var deleted = await _repository.DeleteAsync(id);
            await RemoveCacheAsync(accessToken.Token);
            return deleted;
        }

        public async Task<string> RegenerateAsync(AccessToken accessToken)
        {
            await RemoveCacheAsync(accessToken.Token);
            accessToken.Token = WebConfigUtils.EncryptStringBySecretKey(StringUtils.Guid());

            await _repository.UpdateAsync(accessToken);

            return accessToken.Token;
        }

        public async Task<bool> IsTitleExistsAsync(string title)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(AccessToken.Title), title));
        }

        public async Task<IEnumerable<AccessToken>> GetAccessTokenListAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(nameof(AccessToken.Id)));
        }

        public async Task<AccessToken> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }

        public async Task<bool> IsScopeAsync(string token, string scope)
        {
            if (string.IsNullOrEmpty(token)) return false;

            var tokenInfo = await GetByTokenAsync(token);
            return tokenInfo != null && StringUtils.ContainsIgnoreCase(StringUtils.GetStringList(tokenInfo.Scopes), scope);
        }
    }
}
