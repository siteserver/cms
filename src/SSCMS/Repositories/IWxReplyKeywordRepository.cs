using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IWxReplyKeywordRepository : IRepository
    {
        Task<int> InsertAsync(WxReplyKeyword keyword);

        Task UpdateAsync(WxReplyKeyword keyword);

        Task DeleteAllAsync(int siteId, int ruleId);

        Task<List<WxReplyKeyword>> GetKeywordsAsync(int siteId, int ruleId);

        Task<List<WxReplyKeyword>> GetKeywordsAsync(int siteId);
    }
}
