using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ISiteLogRepository : IRepository
    {
        void Insert(SiteLogInfo logInfo);

        Task DeleteIfThresholdAsync();

        Task DeleteAsync(List<int> idList);

        Task DeleteAllAsync();
    }
}