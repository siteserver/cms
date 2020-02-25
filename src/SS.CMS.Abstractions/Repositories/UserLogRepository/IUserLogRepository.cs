using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface IUserLogRepository : IRepository
    {
        Task InsertAsync(UserLog userLog);

        Task DeleteIfThresholdAsync();

        Task DeleteAllAsync();

        Task<int> GetCountAsync(int userId, string keyword, string dateFrom, string dateTo);

        Task<List<UserLog>> GetAllAsync(int userId, string keyword, string dateFrom, string dateTo, int offset,
            int limit);

        Task<List<UserLog>> GetLogsAsync(int userId, int offset, int limit);

        Task<UserLog> InsertAsync(int userId, UserLog log);
    }
}
