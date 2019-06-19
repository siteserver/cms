using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Enums;

namespace SS.CMS.Core.Common.Create
{
    public static class CreateTaskManager
    {
        private static readonly List<CreateTaskInfo> PendingTasks = new List<CreateTaskInfo>();
        private static readonly List<CreateTaskLogInfo> TaskLogs = new List<CreateTaskLogInfo>();
        private static readonly object LockObject = new object();

        public static void AddPendingTask(CreateTaskInfo task)
        {
            lock (LockObject)
            {
                if (task.CreateType == CreateType.Content)
                {
                    if (PendingTasks.Any(taskInfo =>
                        taskInfo.CreateType == CreateType.AllContent && taskInfo.ChannelId == task.ChannelId))
                    {
                        return;
                    }
                }
                if (PendingTasks.Any(taskInfo =>
                    task.CreateType == taskInfo.CreateType && task.SiteId == taskInfo.SiteId &&
                    task.ChannelId == taskInfo.ChannelId && task.ContentId == taskInfo.ContentId &&
                    task.FileTemplateId == taskInfo.FileTemplateId && task.SpecialId == taskInfo.SpecialId))
                {
                    return;
                }
                PendingTasks.Insert(0, task);
            }
        }

        public static int PendingTaskCount => PendingTasks.Count == 0 ? 0 : PendingTasks.Sum(taskInfo => taskInfo.PageCount);

        public static CreateTaskInfo GetFirstPendingTask()
        {
            lock (LockObject)
            {
                var taskInfo = PendingTasks.FirstOrDefault(task => !task.Executing);
                if (taskInfo == null) return null;
                //var taskInfo = PendingTasks[0];
                taskInfo.Executing = true;
                PendingTasks.Remove(taskInfo);
                PendingTasks.Add(taskInfo);
                return taskInfo;
            }
        }

        public static void RemovePendingTask(CreateTaskInfo taskInfo)
        {
            lock (LockObject)
            {
                PendingTasks.Remove(taskInfo);
            }
        }

        public static void AddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.SiteId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.FileTemplateId, taskInfo.SpecialId, taskInfo.Name, timeSpan, true, string.Empty, DateTime.Now);
            lock (LockObject)
            {
                if (TaskLogs.Count > 20)
                {
                    TaskLogs.RemoveAt(20);
                }
                TaskLogs.Insert(0, taskLog);
            }
        }

        public static void AddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.SiteId, taskInfo.ChannelId, taskInfo.ContentId, taskInfo.FileTemplateId, taskInfo.SpecialId, taskInfo.Name, string.Empty, false, ex.Message, DateTime.Now);
            lock (LockObject)
            {
                if (TaskLogs.Count > 20)
                {
                    TaskLogs.RemoveAt(20);
                }
                TaskLogs.Insert(0, taskLog);
            }
        }

        public static void ClearAllTask(int siteId)
        {
            lock (LockObject)
            {
                var pendingTasks = new List<CreateTaskInfo>();
                foreach (var createTaskInfo in PendingTasks)
                {
                    if (createTaskInfo.SiteId != siteId)
                    {
                        pendingTasks.Add(createTaskInfo);
                    }
                }
                PendingTasks.Clear();
                PendingTasks.AddRange(pendingTasks);
            }
        }

        public static CreateTaskSummary GetTaskSummary(int siteId)
        {
            var list = new List<CreateTaskSummaryItem>();

            var channelsCount = 0;
            var contentsCount = 0;
            var filesCount = 0;
            var specialsCount = 0;

            foreach (var taskInfo in PendingTasks)
            {
                if (taskInfo.SiteId != siteId) continue;

                if (taskInfo.CreateType == CreateType.Channel)
                {
                    channelsCount += taskInfo.PageCount;
                }
                else if (taskInfo.CreateType == CreateType.Content || taskInfo.CreateType == CreateType.AllContent)
                {
                    contentsCount += taskInfo.PageCount;
                }
                else if (taskInfo.CreateType == CreateType.File)
                {
                    filesCount += taskInfo.PageCount;
                }
                else if (taskInfo.CreateType == CreateType.Special)
                {
                    specialsCount += taskInfo.PageCount;
                }

                var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, true, false, string.Empty);
                list.Add(summaryItem);
            }

            //var count = _pendingTasks.Count;
            //if (count > 0)
            //{
            //    foreach (var taskInfo in _pendingTasks)
            //    {
            //        var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, true, false, string.Empty);
            //        list.Add(summaryItem);
            //    }
            //    //var pendingTaskList = _pendingTasks.ToList();
            //    //for (var i = 0; i < count; i++)
            //    //{
            //    //    var taskInfo = pendingTaskList[i];
            //    //    var summaryItem = new CreateTaskSummaryItem(taskInfo, string.Empty, true, false, string.Empty);
            //    //    list.Add(summaryItem);
            //    //}
            //}

            foreach (var logInfo in TaskLogs)
            {
                var summaryItem = new CreateTaskSummaryItem(logInfo);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(list, channelsCount, contentsCount, filesCount, specialsCount);

            return summary;
        }
    }
}
