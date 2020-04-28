using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<AccessToken> _repository;

        public AccessTokenRepository(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<AccessToken>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(AccessToken accessToken)
        {
            var token = _settingsManager.Encrypt(StringUtils.Guid());
            accessToken.Token = token;

            return await _repository.InsertAsync(accessToken);
        }

        public async Task<bool> UpdateAsync(AccessToken accessToken)
        {
            var cacheKey = GetCacheKeyByToken(accessToken.Token);
            return await _repository.UpdateAsync(accessToken, Q.CachingRemove(cacheKey));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var accessToken = await _repository.GetAsync(id);
            if (accessToken == null) return false;

            var cacheKey = GetCacheKeyByToken(accessToken.Token);
            return await _repository.DeleteAsync(Q
                .Where(nameof(AccessToken.Id), id)
                .CachingRemove(cacheKey)
            ) == 1;
        }

        public async Task<string> RegenerateAsync(AccessToken accessToken)
        {
            var cacheKey = GetCacheKeyByToken(accessToken.Token);

            accessToken.Token = _settingsManager.Encrypt(StringUtils.Guid());

            await _repository.UpdateAsync(accessToken, Q.CachingRemove(cacheKey));

            return accessToken.Token;
        }

        public async Task<bool> IsTitleExistsAsync(string title)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(AccessToken.Title), title));
        }

        public async Task<List<AccessToken>> GetAccessTokenListAsync()
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
            return tokenInfo != null && StringUtils.ContainsIgnoreCase(Utilities.GetStringList(tokenInfo.Scopes), scope);
        }
    }
}
