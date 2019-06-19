using System;
using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IErrorLogRepository : IRepository
    {
        void Delete(List<int> idList);

        ErrorLogInfo GetErrorLogInfo(int logId);

        void DeleteAll();

        int AddErrorLog(ErrorLogInfo logInfo);

        int AddErrorLog(Exception ex, string summary = "");

        int AddErrorLog(string pluginId, Exception ex, string summary = "");

        int AddStlErrorLog(string summary, string elementName, string stlContent, Exception ex);
    }
}
