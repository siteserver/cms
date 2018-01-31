using System;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core.Create
{
    public interface ICreateTaskManager
    {
        void AddPendingTask(CreateTaskInfo task);

        int GetPendingTaskCount(int siteId);

        CreateTaskInfo GetAndRemoveLastPendingTask(int siteId);

        void RemoveCurrent(int siteId, CreateTaskInfo taskInfo);

        void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan);

        void AddFailureLog(CreateTaskInfo taskInfo, Exception ex);

        void ClearAllTask();

        void ClearAllTask(int siteId);

        CreateTaskSummary GetTaskSummary(int siteId);
    }
}
