using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface ITemplateLogRepository : IRepository
    {
        Task<int> InsertAsync(TemplateLog logInfo);

        string GetSelectCommend(int siteId, int templateId);

        Task<string> GetTemplateContentAsync(int logId);

        // Dictionary<int, string> GetLogIdWithNameDictionary(int templateId);

        Task DeleteAsync(List<int> idList);
    }
}