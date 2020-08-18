using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class WxReplyRuleRepository : IWxReplyRuleRepository
    {
        private readonly Repository<WxReplyRule> _repository;

        public WxReplyRuleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<WxReplyRule>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(WxReplyRule rule)
        {
            return await _repository.InsertAsync(rule);
        }

        public async Task UpdateAsync(WxReplyRule rule)
        {
            await _repository.UpdateAsync(rule);
        }

        public async Task DeleteAsync(int ruleId)
        {
            await _repository.DeleteAsync(ruleId);
        }

        public async Task<int> GetCount(int siteId, string keyword)
        {
            return await _repository.CountAsync(Q
                .WhereLike(nameof(WxReplyRule.RuleName), $"%{keyword}%")
                .Where(nameof(WxReplyRule.SiteId), siteId)
            );
        }

        public async Task<List<WxReplyRule>> GetRulesAsync(int siteId, string keyword, int page, int perPage)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(WxReplyRule.SiteId), siteId)
                .WhereLike(nameof(WxReplyRule.RuleName), $"%{keyword}%")
                .ForPage(page, perPage)
                .OrderByDesc(nameof(WxReplyRule.Id))
            );
        }

        public async Task<WxReplyRule> GetAsync(int ruleId)
        {
            return await _repository.GetAsync(ruleId);
        }
    }
}
