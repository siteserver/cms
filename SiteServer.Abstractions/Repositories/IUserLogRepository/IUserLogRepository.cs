using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface IUserLogRepository : IRepository
    {
        Task AddUserLogAsync(string ipAddress, int userId, string actionType, string summary);

        Task DeleteAsync(List<int> idList);

        Task DeleteAllAsync();

        Task<int> GetCountAsync();

        Task<IEnumerable<UserLog>> ListAsync(int userId, int totalNum, string action);

        Task<IEnumerable<UserLog>> ApiGetLogsAsync(int userId, int offset, int limit);
    }
}
