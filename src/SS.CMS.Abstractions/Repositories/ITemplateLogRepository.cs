using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ITemplateLogRepository : IRepository
    {
        void Insert(TemplateLogInfo logInfo);

        string GetSelectCommend(int siteId, int templateId);

        string GetTemplateContent(int logId);

        Dictionary<int, string> GetLogIdWithNameDictionary(int templateId);

        void Delete(List<int> idList);
    }
}