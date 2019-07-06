using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface ITemplateLogRepository : IRepository
    {
        void Insert(TemplateLogInfo logInfo);

        string GetSelectCommend(int siteId, int templateId);

        string GetTemplateContent(int logId);

        Dictionary<int, string> GetLogIdWithNameDictionary(int templateId);

        Task DeleteAsync(List<int> idList);
    }
}