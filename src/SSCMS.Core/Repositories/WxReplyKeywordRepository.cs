using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class WxReplyKeywordRepository : IWxReplyKeywordRepository
    {
        private readonly Repository<WxReplyKeyword> _repository;

        public WxReplyKeywordRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<WxReplyKeyword>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId) => CacheUtils.GetListKey(_repository.TableName, siteId);

        public async Task<int> InsertAsync(WxReplyKeyword keyword)
        {
            return await _repository.InsertAsync(keyword, Q
                .CachingRemove(GetCacheKey(keyword.SiteId))
            );
        }

        public async Task UpdateAsync(WxReplyKeyword keyword)
        {
            await _repository.UpdateAsync(keyword, Q
                .CachingRemove(GetCacheKey(keyword.SiteId))
            );
        }

        public async Task DeleteAllAsync(int siteId, int ruleId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxReplyKeyword.SiteId), siteId)
                .Where(nameof(WxReplyKeyword.RuleId), ruleId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<List<WxReplyKeyword>> GetKeywordsAsync(int siteId, int ruleId)
        {
            var allKeywords = await GetKeywordsAsync(siteId);
            return allKeywords.Where(x => x.RuleId == ruleId).ToList();
        }

        public async Task<List<WxReplyKeyword>> GetKeywordsAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(WxReplyKeyword.SiteId), siteId)
                .CachingGet(GetCacheKey(siteId))
            );
        }
    }
}
