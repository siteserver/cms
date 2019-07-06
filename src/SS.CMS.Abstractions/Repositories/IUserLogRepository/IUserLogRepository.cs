using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IUserLogRepository : IRepository
    {
        Task AddUserLogAsync(string ipAddress, string userName, string actionType, string summary);

        Task DeleteAsync(List<int> idList);

        Task DeleteAllAsync();

        int GetCount();

        IList<UserLogInfo> List(string userName, int totalNum, string action);

        IList<UserLogInfo> ApiGetLogs(string userName, int offset, int limit);
    }
}
