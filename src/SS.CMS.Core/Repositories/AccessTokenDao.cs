using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class AccessTokenDao : IDatabaseDao
    {
        private readonly Repository<AccessTokenInfo> _repository;
        public AccessTokenDao()
        {
            _repository = new Repository<AccessTokenInfo>(AppSettings.DbContext);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Title = nameof(AccessTokenInfo.Title);
        }

        public async Task<int> InsertAsync(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());
            accessTokenInfo.AddDate = DateTime.Now;

            accessTokenInfo.Id = await _repository.InsertAsync(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                AccessTokenManager.ClearCache();
            }
            return accessTokenInfo.Id;
        }

        public async Task<bool> UpdateAsync(AccessTokenInfo accessTokenInfo)
        {
            var updated = await _repository.UpdateAsync(accessTokenInfo);
            if (updated)
            {
                AccessTokenManager.ClearCache();
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                AccessTokenManager.ClearCache();
            }
            return deleted;
        }

        public async Task<string> RegenerateAsync(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());

            await UpdateAsync(accessTokenInfo);

            return accessTokenInfo.Token;
        }

        public async Task<bool> IsTitleExistsAsync(string title)
        {
            return await _repository.ExistsAsync(Q.Where(Attr.Title, title));
        }

        public IList<AccessTokenInfo> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public async Task<IEnumerable<AccessTokenInfo>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<AccessTokenInfo> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
    }
}
