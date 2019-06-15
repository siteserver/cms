using System;
using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IErrorLogRepository : IRepository
    {
        void Delete(List<int> idList);

        ErrorLogInfo GetErrorLogInfo(int logId);

        void DeleteAll();

        int AddErrorLog(ErrorLogInfo logInfo);

        int AddErrorLog(Exception ex, string summary = "");

        int AddErrorLog(string pluginId, Exception ex, string summary = "");
    }
}
