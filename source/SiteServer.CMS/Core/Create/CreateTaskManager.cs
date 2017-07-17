using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public interface ICreateTaskManager
    {
        void AddPendingTask(CreateTaskInfo task);

        int GetPendingTaskCount(int publishmentSystemId);

        CreateTaskInfo GetLastPendingTask(int publishmentSystemId);

        void RemoveTask(int publishmentSystemId, CreateTaskInfo taskInfo);

        void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan);

        void AddFailureLog(CreateTaskInfo taskInfo, Exception ex);

        void ClearAllTask();

        void ClearAllTask(int publishmentSystemId);

        CreateTaskSummary GetTaskSummary(int publishmentSystemId);
    }

    public class CreateTaskManager
    {
        private static readonly CreateTaskManagerForDb CreateTaskManagerForDb = new CreateTaskManagerForDb();
        private static readonly CreateTaskManagerForMomery CreateTaskManagerForMomery = new CreateTaskManagerForMomery();

        private CreateTaskManager() { }

        public static ICreateTaskManager Instance => ServiceManager.IsServiceOnline()
            ? (ICreateTaskManager) CreateTaskManagerForDb
            : CreateTaskManagerForMomery;
    }

    internal class CreateTaskManagerForMomery : ICreateTaskManager
    {
        private static readonly Dictionary<int, List<CreateTaskInfo>> PendingTaskDict = new Dictionary<int, List<CreateTaskInfo>>();
        private static readonly Dictionary<int, List<CreateTaskLogInfo>> TaskLogDict = new Dictionary<int, List<CreateTaskLogInfo>>();

        /// <summary>
        /// 获取某个站点的所有任务
        /// </summary>
        /// <param name="publishmentSystemId"></param>
        /// <returns></returns>
        private List<CreateTaskInfo> GetPendingTasks(int publishmentSystemId)
        {
            if (!PendingTaskDict.ContainsKey(publishmentSystemId))
            {
                PendingTaskDict.Add(publishmentSystemId, new List<CreateTaskInfo>());
            }
            return PendingTaskDict[publishmentSystemId];
        }

        private List<CreateTaskLogInfo> GetTaskLogs(int publishmentSystemId)
        {
            if (!TaskLogDict.ContainsKey(publishmentSystemId))
            {
                TaskLogDict.Add(publishmentSystemId, new List<CreateTaskLogInfo>());
            }
            return TaskLogDict[publishmentSystemId];
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="task"></param>
        public void AddPendingTask(CreateTaskInfo task)
        {
            var pendingTasks = GetPendingTasks(task.PublishmentSystemId); // 查找某站点所有任务
            foreach (var taskInfo in pendingTasks)
            {
                if (task.Equals(taskInfo))
                {
                    return;
                }
            }
            pendingTasks.Add(task);
        }

        public int GetPendingTaskCount(int publishmentSystemId)
        {
            var pendingTasks = GetPendingTasks(publishmentSystemId);
            return pendingTasks.Count == 0 ? 0 : pendingTasks.Sum(taskInfo => taskInfo.PageCount);
        }

        public CreateTaskInfo GetLastPendingTask(int publishmentSystemId)
        {
            var pendingTasks = GetPendingTasks(publishmentSystemId);
            if (pendingTasks.Count <= 0) return null;

            return pendingTasks[0];
        }

        public void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var taskLogs = GetTaskLogs(taskInfo.PublishmentSystemId);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.Name, timeSpan, true, string.Empty, DateTime.Now);
            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void AddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var taskLogs = GetTaskLogs(taskInfo.PublishmentSystemId);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.Name, string.Empty, false, ex.Message, DateTime.Now);
            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void ClearAllTask()
        {
            foreach (var publishmentSystemId in PendingTaskDict.Keys)
            {
                PendingTaskDict[publishmentSystemId] = new List<CreateTaskInfo>();
            }
        }

        public void ClearAllTask(int publishmentSystemId)
        {
            PendingTaskDict[publishmentSystemId] = new List<CreateTaskInfo>();
        }

        public void RemoveTask(int publishmentSystemId, CreateTaskInfo taskInfo)
        {
            var pendingTasks = GetPendingTasks(publishmentSystemId);
            pendingTasks.Remove(taskInfo);
        }

        public CreateTaskSummary GetTaskSummary(int publishmentSystemId)
        {
            var pendingTasks = GetPendingTasks(publishmentSystemId);
            var taskLogs = GetTaskLogs(publishmentSystemId);

            var list = new List<CreateTaskSummaryItem>();

            var channelsCount = 0;
            var contentsCount = 0;
            var filesCount = 0;

            foreach (var taskInfo in pendingTasks)
            {
                if (taskInfo.CreateType == ECreateType.Channel)
                {
                    channelsCount += taskInfo.PageCount;
                }
                else if (taskInfo.CreateType == ECreateType.Content || taskInfo.CreateType == ECreateType.AllContent)
                {
                    contentsCount += taskInfo.PageCount;
                }
                else if (taskInfo.CreateType == ECreateType.File)
                {
                    filesCount += taskInfo.PageCount;
                }
            }
            
            CreateTaskSummaryItem current = null;
            var count = pendingTasks.Count >= 11 ? 11 : pendingTasks.Count;
            if (count > 0)
            {
                current = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(pendingTasks[0].CreateType), pendingTasks[0].Name, string.Empty, false, false, string.Empty);

                for (var i = 1; i < count; i++)
                {
                    var taskInfo = pendingTasks[i];
                    var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(taskInfo.CreateType), taskInfo.Name, string.Empty, false, false, string.Empty);
                    list.Add(summaryItem);
                }
            }

            count = taskLogs.Count >= 20 ? 20 : taskLogs.Count;
            for (var i = 1; i <= count; i++)
            {
                var logInfo = taskLogs[taskLogs.Count - i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(logInfo.CreateType), logInfo.TaskName, logInfo.TimeSpan, true, logInfo.IsSuccess, logInfo.ErrorMessage);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(current, list, channelsCount, contentsCount, filesCount);

            return summary;
        }
    }

    internal class CreateTaskManagerForDb: ICreateTaskManager
    {
        public void AddPendingTask(CreateTaskInfo task)
        {
            if (!DataProvider.CreateTaskDao.IsExists(task))
            {
                DataProvider.CreateTaskDao.Insert(task);
            }
        }

        public CreateTaskInfo GetLastPendingTask(int publishmentSystemId)
        {
            return DataProvider.CreateTaskDao.GetLastPendingTask();
        }

        //public List<CreateTaskInfo> GetLastPendingTasks(int publishmentSystemId, int topNum)
        //{
        //    return DataProvider.CreateTaskDao.GetLastPendingTasks(topNum);
        //}

        public void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.Name, timeSpan, true, string.Empty, DateTime.Now);
            DataProvider.CreateTaskLogDao.Insert(taskLog);
        }

        public void AddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.Name, string.Empty, false, ex.Message, DateTime.Now);
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

            CreateTaskSummaryItem current = null;
            var list = new List<CreateTaskSummaryItem>();

            for (var i = pendingTasks.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    current = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(pendingTasks[0].CreateType), pendingTasks[0].Name, string.Empty, false, false, string.Empty);
                }
                var taskInfo = pendingTasks[i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(taskInfo.CreateType), taskInfo.Name, string.Empty, false, false, string.Empty);
                list.Add(summaryItem);
            }

            for (var i = taskLogs.Count - 1; i >= 0; i--)
            {
                var logInfo = taskLogs[i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(logInfo.CreateType), logInfo.TaskName, logInfo.TimeSpan, true, logInfo.IsSuccess, logInfo.ErrorMessage);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(current, list, channelsCount, contentsCount, filesCount);

            return summary;
        }

        public int GetPendingTaskCount(int publishmentSystemId)
        {
            return 0;
        }

        public void RemoveTask(int publishmentSystemId, CreateTaskInfo taskInfo)
        {
            DataProvider.CreateTaskDao.Delete(taskInfo.Id);
        }
    }
}
