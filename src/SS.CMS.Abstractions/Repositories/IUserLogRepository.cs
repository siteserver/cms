using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IUserLogRepository : IRepository
    {
        void AddUserLog(string ipAddress, string userName, string actionType, string summary);

        void Delete(List<int> idList);

        void DeleteAll();

        int GetCount();

        IList<UserLogInfo> List(string userName, int totalNum, string action);

        IList<UserLogInfo> ApiGetLogs(string userName, int offset, int limit);
    }
}
