using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public interface ITemplateLogRepository : IRepository
    {
        Task InsertAsync(TemplateLog log);

        Task<string> GetTemplateContentAsync(int logId);

        Task<List<KeyValuePair<int, string>>> GetLogIdWithNameListAsync(int siteId, int templateId);

        Task DeleteAsync(int logId);
    }
}