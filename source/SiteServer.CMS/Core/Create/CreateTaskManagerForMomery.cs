using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    internal class CreateTaskManagerForMomery : ICreateTaskManager
    {
        private static readonly ConcurrentDictionary<int, ConcurrentQueue<CreateTaskInfo>> PendingTaskDict = new ConcurrentDictionary<int, ConcurrentQueue<CreateTaskInfo>>();
        private static readonly ConcurrentDictionary<int, List<CreateTaskLogInfo>> TaskLogDict = new ConcurrentDictionary<int, List<CreateTaskLogInfo>>();
        private static readonly object LockObject = new object();

        /// <summary>
        /// 获取某个站点的所有任务
        /// </summary>
        /// <param name="publishmentSystemId"></param>
        /// <returns></returns>
        private static ConcurrentQueue<CreateTaskInfo> GetPendingTasks(int publishmentSystemId)
        {
            lock (LockObject)
            {
                if (!PendingTaskDict.ContainsKey(publishmentSystemId))
                {
                    PendingTaskDict.TryAdd(publishmentSystemId, new ConcurrentQueue<CreateTaskInfo>());
                }
                return PendingTaskDict[publishmentSystemId];
            }
        }

        private static List<CreateTaskLogInfo> GetTaskLogs(int publishmentSystemId)
        {
            lock (LockObject)
            {
                if (!TaskLogDict.ContainsKey(publishmentSystemId))
                {
                    TaskLogDict.TryAdd(publishmentSystemId, new List<CreateTaskLogInfo>());
                }
                return TaskLogDict[publishmentSystemId];
            }
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
            pendingTasks.Enqueue(task);
        }

        public int GetPendingTaskCount(int publishmentSystemId)
        {
            var pendingTasks = GetPendingTasks(publishmentSystemId);
            return pendingTasks.Count == 0 ? 0 : pendingTasks.Sum(taskInfo => taskInfo.PageCount);
        }

        public CreateTaskInfo GetAndRemoveLastPendingTask(int publishmentSystemId)
        {
            lock (LockObject)
            {
                var pendingTasks = GetPendingTasks(publishmentSystemId);
                if (pendingTasks.Count <= 0) return null;
                CreateTaskInfo taskInfo;
                pendingTasks.TryDequeue(out taskInfo);

                return taskInfo;
            }
        }

        public void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var taskLogs = GetTaskLogs(taskInfo.PublishmentSystemId);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.TemplateId, taskInfo.Name, timeSpan, true, string.Empty, DateTime.Now);
            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void AddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var taskLogs = GetTaskLogs(taskInfo.PublishmentSystemId);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.TemplateId, taskInfo.Name, string.Empty, false, ex.Message, DateTime.Now);
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
                PendingTaskDict[publishmentSystemId] = new ConcurrentQueue<CreateTaskInfo>();
            }
        }

        public void ClearAllTask(int publishmentSystemId)
        {
            PendingTaskDict[publishmentSystemId] = new ConcurrentQueue<CreateTaskInfo>();
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
                var pendingTaskList = pendingTasks.ToList();
                var pendingTask = pendingTaskList[0];
                current = new CreateTaskSummaryItem(pendingTask, string.Empty, false, false, string.Empty);

                for (var i = 1; i < count; i++)
                {
                    var taskInfo = pendingTaskList[i];
                    var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, false, false, string.Empty);
                    list.Add(summaryItem);
                }
            }

            count = taskLogs.Count >= 20 ? 20 : taskLogs.Count;
            for (var i = 1; i <= count; i++)
            {
                var logInfo = taskLogs[taskLogs.Count - i];
                var summaryItem = new CreateTaskSummaryItem(logInfo, true);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(current, list, channelsCount, contentsCount, filesCount);

            return summary;
        }
    }
}
