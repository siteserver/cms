using System;
using System.Collections.Generic;
using System.Linq;
using SSCMS;
using SSCMS.Dto.Create;

namespace SSCMS.Core.Services.CreateManager
{
    public partial class CreateManager
    {
        private static readonly List<CreateTask> PendingTasks = new List<CreateTask>();
        private static readonly List<CreateTaskLog> TaskLogs = new List<CreateTaskLog>();
        private static readonly object LockObject = new object();

        public void AddPendingTask(CreateTask task)
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

        public int PendingTaskCount => PendingTasks.Count == 0 ? 0 : PendingTasks.Sum(taskInfo => taskInfo.PageCount);

        public CreateTask GetFirstPendingTask()
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

        public void RemovePendingTask(CreateTask task)
        {
            lock (LockObject)
            {
                PendingTasks.Remove(task);
            }
        }

        public void AddSuccessLog(CreateTask task, string timeSpan)
        {
            var taskLog = new CreateTaskLog(0, task.CreateType, task.SiteId, task.ChannelId, task.ContentId, task.FileTemplateId, task.SpecialId, task.Name, timeSpan, true, string.Empty, DateTime.Now);
            lock (LockObject)
            {
                if (TaskLogs.Count > 20)
                {
                    TaskLogs.RemoveAt(20);
                }
                TaskLogs.Insert(0, taskLog);
            }
        }

        public void AddFailureLog(CreateTask task, Exception ex)
        {
            var taskLog = new CreateTaskLog(0, task.CreateType, task.SiteId, task.ChannelId, task.ContentId, task.FileTemplateId, task.SpecialId, task.Name, string.Empty, false, ex.Message, DateTime.Now);
            lock (LockObject)
            {
                if (TaskLogs.Count > 20)
                {
                    TaskLogs.RemoveAt(20);
                }
                TaskLogs.Insert(0, taskLog);
            }
        }

        public void ClearAllTask(int siteId)
        {
            lock (LockObject)
            {
                var pendingTasks = new List<CreateTask>();
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

        public CreateTaskSummary GetTaskSummary(int siteId)
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
