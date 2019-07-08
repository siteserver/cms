using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IUserLogRepository : IRepository
    {
        Task AddUserLogAsync(string ipAddress, int userId, string actionType, string summary);

        Task DeleteAsync(List<int> idList);

        Task DeleteAllAsync();

        int GetCount();

        IList<UserLogInfo> List(int userId, int totalNum, string action);

        IList<UserLogInfo> ApiGetLogs(int userId, int offset, int limit);
    }
}
