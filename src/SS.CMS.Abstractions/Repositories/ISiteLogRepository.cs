using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ISiteLogRepository : IRepository
    {
        void Insert(SiteLogInfo logInfo);

        void DeleteIfThreshold();

        void Delete(List<int> idList);

        void DeleteAll();
    }
}