using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public class CreateTaskManager : ICreateTaskManager
    {
        private static readonly ConcurrentDictionary<int, ConcurrentQueue<CreateTaskInfo>> PendingTaskDict = new ConcurrentDictionary<int, ConcurrentQueue<CreateTaskInfo>>();
        private static readonly ConcurrentDictionary<int, List<CreateTaskInfo>> ExecutingTaskDict = new ConcurrentDictionary<int, List<CreateTaskInfo>>();
        private static readonly ConcurrentDictionary<int, List<CreateTaskLogInfo>> TaskLogDict = new ConcurrentDictionary<int, List<CreateTaskLogInfo>>();
        private static readonly object LockObject = new object();

        public static CreateTaskManager Instance { get; } = new CreateTaskManager();

        /// <summary>
        /// 获取某个站点的所有任务
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        private static ConcurrentQueue<CreateTaskInfo> GetPendingTasks(int siteId)
        {
            lock (LockObject)
            {
                if (!PendingTaskDict.ContainsKey(siteId))
                {
                    PendingTaskDict.TryAdd(siteId, new ConcurrentQueue<CreateTaskInfo>());
                }
                return PendingTaskDict[siteId];
            }
        }

        private static List<CreateTaskInfo> GetExecutingTasks(int siteId)
        {
            lock (LockObject)
            {
                if (!ExecutingTaskDict.ContainsKey(siteId))
                {
                    ExecutingTaskDict.TryAdd(siteId, new List<CreateTaskInfo>());
                }
                return ExecutingTaskDict[siteId];
            }
        }

        private static List<CreateTaskLogInfo> GetTaskLogs(int siteId)
        {
            lock (LockObject)
            {
                if (!TaskLogDict.ContainsKey(siteId))
                {
                    TaskLogDict.TryAdd(siteId, new List<CreateTaskLogInfo>());
                }
                return TaskLogDict[siteId];
            }
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="task"></param>
        public void AddPendingTask(CreateTaskInfo task)
        {
            var pendingTasks = GetPendingTasks(task.SiteId); // 查找某站点所有任务
            foreach (var taskInfo in pendingTasks)
            {
                if (task.Equals(taskInfo))
                {
                    return;
                }
            }
            pendingTasks.Enqueue(task);
        }

        public int GetPendingTaskCount(int siteId)
        {
            var pendingTasks = GetPendingTasks(siteId);
            return pendingTasks.Count == 0 ? 0 : pendingTasks.Sum(taskInfo => taskInfo.PageCount);
        }

        public CreateTaskInfo GetAndRemoveLastPendingTask(int siteId)
        {
            lock (LockObject)
            {
                var pendingTasks = GetPendingTasks(siteId);
                CreateTaskInfo taskInfo;
                pendingTasks.TryDequeue(out taskInfo);

                if (taskInfo != null)
                {
                    var executingTasks = GetExecutingTasks(siteId);
                    executingTasks.Add(taskInfo);
                }

                return taskInfo;
            }
        }

        public void RemoveCurrent(int siteId, CreateTaskInfo taskInfo)
        {
            lock (LockObject)
            {
                var executingTasks = GetExecutingTasks(siteId);
                executingTasks.Remove(taskInfo);
            }
        }

        public void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var taskLogs = GetTaskLogs(taskInfo.SiteId);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.SiteId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.FileTemplateId, taskInfo.SpecialId, taskInfo.Name, timeSpan, true, string.Empty, DateTime.Now);
            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void AddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var taskLogs = GetTaskLogs(taskInfo.SiteId);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.SiteId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.FileTemplateId, taskInfo.SpecialId, taskInfo.Name, string.Empty, false, ex.Message, DateTime.Now);
            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void ClearAllTask()
        {
            foreach (var siteId in PendingTaskDict.Keys)
            {
                PendingTaskDict[siteId] = new ConcurrentQueue<CreateTaskInfo>();
            }
        }

        public void ClearAllTask(int siteId)
        {
            PendingTaskDict[siteId] = new ConcurrentQueue<CreateTaskInfo>();
        }

        public CreateTaskSummary GetTaskSummary(int siteId)
        {
            var executingTasks = GetExecutingTasks(siteId);
            var pendingTasks = GetPendingTasks(siteId);
            var taskLogs = GetTaskLogs(siteId);

            var list = new List<CreateTaskSummaryItem>();

            var channelsCount = 0;
            var contentsCount = 0;
            var filesCount = 0;
            var specialsCount = 0;

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
                else if (taskInfo.CreateType == ECreateType.Special)
                {
                    specialsCount += taskInfo.PageCount;
                }
            }

            if (executingTasks.Count > 0)
            {
                foreach (var taskInfo in executingTasks)
                {
                    var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, true, false, false, string.Empty);
                    list.Add(summaryItem);
                }
            }

            var count = pendingTasks.Count >= 10 ? 10 : pendingTasks.Count;
            if (count > 0)
            {
                var pendingTaskList = pendingTasks.ToList();
                for (var i = 0; i < count; i++)
                {
                    var taskInfo = pendingTaskList[i];
                    var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, false, true, false, string.Empty);
                    list.Add(summaryItem);
                }
            }

            count = taskLogs.Count >= 20 ? 20 : taskLogs.Count;
            for (var i = 1; i <= count; i++)
            {
                var logInfo = taskLogs[taskLogs.Count - i];
                var summaryItem = new CreateTaskSummaryItem(logInfo);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(list, channelsCount, contentsCount, filesCount, specialsCount);

            return summary;
        }
    }
}
