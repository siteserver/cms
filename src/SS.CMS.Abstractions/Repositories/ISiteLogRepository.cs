using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ISiteLogRepository : IRepository
    {
        void Insert(SiteLogInfo logInfo);

        void DeleteIfThreshold();

        void Delete(List<int> idList);

        void DeleteAll();
    }
}