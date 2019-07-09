using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ISiteLogRepository : IRepository
    {
        Task<int> InsertAsync(SiteLogInfo logInfo);

        Task DeleteIfThresholdAsync();

        Task DeleteAsync(List<int> idList);

        Task DeleteAllAsync();
    }
}