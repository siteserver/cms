using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface ILogRepository : IRepository
    {
        Task AddAdminLogAsync(Administrator admin, string ipAddress, string action, string summary = "");

        Task AddUserLogAsync(User user, string ipAddress, string action, string summary = "");

        Task DeleteIfThresholdAsync();

        Task DeleteAllAdminLogsAsync();

        Task DeleteAllUserLogsAsync();

        Task<int> GetAdminLogsCountAsync(int adminId, string keyword, string dateFrom, string dateTo);

        Task<List<Log>> GetAdminLogsAsync(int adminId, string keyword, string dateFrom, string dateTo, int offset, int limit);

        Task<int> GetUserLogsCountAsync(int userId, string keyword, string dateFrom, string dateTo);

        Task<List<Log>> GetUserLogsAsync(int userId, string keyword, string dateFrom, string dateTo, int offset, int limit);

        Task<List<Log>> GetUserLogsAsync(int userId, int offset, int limit);

        Task<List<Log>> GetAdminLogsAsync(int adminId, int offset, int limit);
    }
}