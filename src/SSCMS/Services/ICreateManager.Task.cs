using SSCMS.Dto;

namespace SSCMS.Services
{
    partial interface ICreateManager
    {
        void AddPendingTask(CreateTask task);

        int PendingTaskCount { get; }

        //CreateTask GetFirstPendingTask();

        //void RemovePendingTask(CreateTask task);

        //void AddSuccessLog(CreateTask task, string timeSpan);

        //void AddFailureLog(CreateTask task, Exception ex);

        void ClearAllTask(int siteId);

        CreateTaskSummary GetTaskSummary(int siteId);
    }
}
