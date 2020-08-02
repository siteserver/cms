using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxReplyRuleRepository : IRepository
    {
        Task<int> InsertAsync(WxReplyRule rule);

        Task UpdateAsync(WxReplyRule rule);

        Task DeleteAsync(int ruleId);

        Task<List<WxReplyRule>> GetRulesAsync(int siteId);

        Task<WxReplyRule> GetAsync(int ruleId);
    }
}
