using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    public partial interface ILogRepository : IRepository
    {
        Task InsertAsync(Log log);

        Task DeleteIfThresholdAsync();

        Task DeleteAllAsync();

        Task<int> GetCountAsync(int adminId, string keyword, string dateFrom, string dateTo);

        Task<List<Log>> GetAllAsync(int adminId, string keyword, string dateFrom, string dateTo, int offset, int limit);

        Dictionary<DateTime, int> GetAdminLoginDictionaryByDate(DateTime dateFrom, DateTime dateTo, string xType, string actionType);

        Task<Dictionary<string, int>> GetAdminLoginDictionaryByNameAsync(DateTime dateFrom, DateTime dateTo,
            string actionType);
    }
}