using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface ISiteLogRepository : IRepository
    {
        Task<int> InsertAsync(SiteLog logInfo);

        Task DeleteIfThresholdAsync();

        Task DeleteAsync(List<int> idList);

        Task DeleteAllAsync();
    }
}