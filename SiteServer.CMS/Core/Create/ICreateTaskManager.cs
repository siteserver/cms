using System;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core.Create
{
    public interface ICreateTaskManager
    {
        void AddPendingTask(CreateTaskInfo task);

        int GetPendingTaskCount(int publishmentSystemId);

        CreateTaskInfo GetAndRemoveLastPendingTask(int publishmentSystemId);

        void RemoveCurrent(int publishmentSystemId, CreateTaskInfo taskInfo);

        void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan);

        void AddFailureLog(CreateTaskInfo taskInfo, Exception ex);

        void ClearAllTask();

        void ClearAllTask(int publishmentSystemId);

        CreateTaskSummary GetTaskSummary(int publishmentSystemId);
    }
}
