using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ILogRepository : IRepository
    {
        int Insert(LogInfo log);

        void Delete(List<int> idList);

        void DeleteIfThreshold();

        void DeleteAll();

        int GetCount();

        DateTime GetLastRemoveLogDate();

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