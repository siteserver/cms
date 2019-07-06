using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ILogRepository : IRepository
    {
        int Insert(LogInfo log);

        Task DeleteAsync(List<int> idList);

        Task DeleteIfThresholdAsync();

        Task DeleteAllAsync();

        int GetCount();

        DateTimeOffset GetLastRemoveLogDate();

        // /// <summary>
        // /// 统计管理员actionType的操作次数
        // /// </summary>
        // /// <param name="dateFrom"></param>
        // /// <param name="dateTo"></param>
        // /// <param name="xType"></param>
        // /// <param name="actionType"></param>
        // /// <returns></returns>
        // Dictionary<DateTime, int> GetAdminLoginDictionaryByDate(DateTime dateFrom, DateTime dateTo, string xType, string actionType);

        // /// <summary>
        // /// 统计管理员actionType的操作次数
        // /// </summary>
        // Dictionary<string, int> GetAdminLoginDictionaryByName(DateTime dateFrom, DateTime dateTo, string actionType);
    }
}