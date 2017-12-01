using System;
using System.Collections.Generic;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core.Create
{
    internal class CreateTaskManagerForDb: ICreateTaskManager
    {
        private static readonly object LockObject = new object();

        public void AddPendingTask(CreateTaskInfo task)
        {
            if (!DataProvider.CreateTaskDao.IsExists(task))
            {
                DataProvider.CreateTaskDao.Insert(task);
            }
        }

        public CreateTaskInfo GetAndRemoveLastPendingTask(int publishmentSystemId)
        {
            lock (LockObject)
            {
                var taskInfo = DataProvider.CreateTaskDao.GetLastPendingTask();
                if (taskInfo != null)
                {
                    DataProvider.CreateTaskDao.Delete(taskInfo.Id);
                }

                return taskInfo;
            }
        }

        public void RemoveCurrent(int publishmentSystemId, CreateTaskInfo taskInfo)
        {
            
        }

        public void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.TemplateId, taskInfo.Name, timeSpan, true, string.Empty, DateTime.Now);
            DataProvider.CreateTaskLogDao.Insert(taskLog);
        }

        public void AddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.TemplateId, taskInfo.Name, string.Empty, false, ex.Message, DateTime.Now);
            DataProvider.CreateTaskLogDao.Insert(taskLog);
        }

        public void ClearAllTask()
        {
            DataProvider.CreateTaskDao.DeleteAll();
        }

        public void ClearAllTask(int publishmentSystemId)
        {
            DataProvider.CreateTaskDao.DeleteByPublishmentSystemId(publishmentSystemId);
        }

        public CreateTaskSummary GetTaskSummary(int publishmentSystemId)
        {
            int channelsCount;
            int contentsCount;
            int filesCount;
            DataProvider.CreateTaskDao.GetCount(publishmentSystemId, out channelsCount, out contentsCount, out filesCount);
            var pendingTasks = DataProvider.CreateTaskDao.GetList(publishmentSystemId, 10);
            var taskLogs = DataProvider.CreateTaskLogDao.GetList(publishmentSystemId, 20);

            var list = new List<CreateTaskSummaryItem>();

            for (var i = pendingTasks.Count - 1; i >= 0; i--)
            {
                var taskInfo = pendingTasks[i];
                var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, false, true, false, string.Empty);
                list.Add(summaryItem);
            }

            for (var i = taskLogs.Count - 1; i >= 0; i--)
            {
                var logInfo = taskLogs[i];
                var summaryItem = new CreateTaskSummaryItem(logInfo);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(list, channelsCount, contentsCount, filesCount);

            return summary;
        }

        public int GetPendingTaskCount(int publishmentSystemId)
        {
            return 0;
        }
    }
}
