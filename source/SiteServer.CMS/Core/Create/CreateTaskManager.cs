using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public interface ICreateTaskManager
    {
        void AddPendingTask(CreateTaskInfo task);

        CreateTaskInfo GetLastPendingTask();

        void RemovePendingAndAddSuccessLog(CreateTaskInfo taskInfo, string timeSpan);

        void RemovePendingAndAddFailureLog(CreateTaskInfo taskInfo, Exception ex);

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

        internal static string GetTaskName(CreateTaskInfo taskInfo)
        {
            var name = string.Empty;
            if (taskInfo.CreateType == ECreateType.Index)
            {
                name = "首页";
            }
            else if (taskInfo.CreateType == ECreateType.Channel)
            {
                name = NodeManager.GetNodeName(taskInfo.PublishmentSystemID, taskInfo.ChannelID);
            }
            else if (taskInfo.CreateType == ECreateType.AllContent)
            {
                var nodeInfo = NodeManager.GetNodeInfo(taskInfo.PublishmentSystemID, taskInfo.ChannelID);
                name = $"{nodeInfo.NodeName}下所有内容页，共{nodeInfo.ContentNum}项";
            }
            else if (taskInfo.CreateType == ECreateType.Content)
            {
                name = BaiRongDataProvider.ContentDao.GetValue(NodeManager.GetTableName(PublishmentSystemManager.GetPublishmentSystemInfo(taskInfo.PublishmentSystemID), taskInfo.ChannelID), taskInfo.ContentID, ContentAttribute.Title);
            }
            else if (taskInfo.CreateType == ECreateType.File)
            {
                name = TemplateManager.GetTemplateName(taskInfo.PublishmentSystemID, taskInfo.TemplateID);
            }
            return name;
        }
    }

    internal class CreateTaskManagerForMomery : ICreateTaskManager
    {
        private static readonly Dictionary<int, List<CreateTaskInfo>> PendingTaskDict = new Dictionary<int, List<CreateTaskInfo>>();
        private static readonly Dictionary<int, List<CreateTaskLogInfo>> TaskLogDict = new Dictionary<int, List<CreateTaskLogInfo>>();

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

        public void AddPendingTask(CreateTaskInfo task)
        {
            var pendingTasks = GetPendingTasks(task.PublishmentSystemID);
            foreach (var taskInfo in pendingTasks)
            {
                if (task.Equals(taskInfo))
                {
                    return;
                }
            }
            pendingTasks.Add(task);
        }

        public CreateTaskInfo GetLastPendingTask()
        {
            foreach (var entry in PendingTaskDict)
            {
                var pendingTasks = entry.Value;
                if (pendingTasks.Count > 0)
                {
                    return pendingTasks[pendingTasks.Count - 1];
                }
            }
            return null;
        }

        public void RemovePendingAndAddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            var pendingTasks = GetPendingTasks(taskInfo.PublishmentSystemID);
            var taskLogs = GetTaskLogs(taskInfo.PublishmentSystemID);

            pendingTasks.Remove(taskInfo);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemID, CreateTaskManager.GetTaskName(taskInfo), timeSpan, true, string.Empty, DateTime.Now);

            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void RemovePendingAndAddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            var pendingTasks = GetPendingTasks(taskInfo.PublishmentSystemID);
            var taskLogs = GetTaskLogs(taskInfo.PublishmentSystemID);

            pendingTasks.Remove(taskInfo);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemID, CreateTaskManager.GetTaskName(taskInfo), string.Empty, false, ex.Message, DateTime.Now);
            if (taskLogs.Count > 20)
            {
                taskLogs.RemoveAt(20);
            }
            taskLogs.Add(taskLog);
        }

        public void ClearAllTask(int publishmentSystemId)
        {
            PendingTaskDict[publishmentSystemId] = new List<CreateTaskInfo>();
        }

        public CreateTaskSummary GetTaskSummary(int publishmentSystemId)
        {
            var pendingTasks = GetPendingTasks(publishmentSystemId);
            var taskLogs = GetTaskLogs(publishmentSystemId);

            var list = new List<CreateTaskSummaryItem>();

            var indexCount = 0;
            var channelsCount = 0;
            var contentsCount = 0;
            var filesCount = 0;

            foreach (var taskInfo in pendingTasks)
            {
                if (taskInfo.CreateType == ECreateType.Index)
                {
                    indexCount++;
                }
                else if (taskInfo.CreateType == ECreateType.Channel)
                {
                    channelsCount++;
                }
                else if (taskInfo.CreateType == ECreateType.Content)
                {
                    contentsCount++;
                }
                else if (taskInfo.CreateType == ECreateType.File)
                {
                    filesCount++;
                }
                else if (taskInfo.CreateType == ECreateType.AllContent)
                {
                    contentsCount++;
                    //NodeInfo nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, taskInfo.ChannelID);
                    //contentsCount += nodeInfo.ContentNum;
                }
            }

            var count = 0;
            for (var i = pendingTasks.Count - 1; i >= 0; i--)
            {
                if (count > 10) break;
                var taskInfo = pendingTasks[i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(taskInfo.CreateType), CreateTaskManager.GetTaskName(taskInfo), string.Empty, false, false, string.Empty);
                list.Add(summaryItem);
            }

            count = 0;
            for (var i = taskLogs.Count - 1; i >= 0; i--)
            {
                if (count > 20) break;
                var logInfo = taskLogs[i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(logInfo.CreateType), logInfo.TaskName, logInfo.TimeSpan, true, logInfo.IsSuccess, logInfo.ErrorMessage);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(list, indexCount, channelsCount, contentsCount, filesCount);

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

        public CreateTaskInfo GetLastPendingTask()
        {
            return DataProvider.CreateTaskDao.GetLastPendingTask();
        }

        public void RemovePendingAndAddSuccessLog(CreateTaskInfo taskInfo, string timeSpan)
        {
            DataProvider.CreateTaskDao.Delete(taskInfo.ID);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemID, CreateTaskManager.GetTaskName(taskInfo), timeSpan, true, string.Empty, DateTime.Now);
            DataProvider.CreateTaskLogDao.Insert(taskLog);
        }

        public void RemovePendingAndAddFailureLog(CreateTaskInfo taskInfo, Exception ex)
        {
            DataProvider.CreateTaskDao.Delete(taskInfo.ID);
            var taskLog = new CreateTaskLogInfo(0, taskInfo.CreateType, taskInfo.PublishmentSystemID, CreateTaskManager.GetTaskName(taskInfo), string.Empty, false, ex.Message, DateTime.Now);
            DataProvider.CreateTaskLogDao.Insert(taskLog);
        }

        public void ClearAllTask(int publishmentSystemId)
        {
            DataProvider.CreateTaskDao.DeleteByPublishmentSystemId(publishmentSystemId);
        }

        public CreateTaskSummary GetTaskSummary(int publishmentSystemId)
        {
            int indexCount;
            int channelsCount;
            int contentsCount;
            int filesCount;
            DataProvider.CreateTaskDao.GetCount(publishmentSystemId, out indexCount, out channelsCount, out contentsCount, out filesCount);
            var pendingTasks = DataProvider.CreateTaskDao.GetList(publishmentSystemId, 10);
            var taskLogs = DataProvider.CreateTaskLogDao.GetList(publishmentSystemId, 20);

            var list = new List<CreateTaskSummaryItem>();

            for (var i = pendingTasks.Count - 1; i >= 0; i--)
            {
                var taskInfo = pendingTasks[i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(taskInfo.CreateType), CreateTaskManager.GetTaskName(taskInfo), string.Empty, false, false, string.Empty);
                list.Add(summaryItem);
            }

            for (var i = taskLogs.Count - 1; i >= 0; i--)
            {
                var logInfo = taskLogs[i];
                var summaryItem = new CreateTaskSummaryItem(ECreateTypeUtils.GetText(logInfo.CreateType), logInfo.TaskName, logInfo.TimeSpan, true, logInfo.IsSuccess, logInfo.ErrorMessage);
                list.Add(summaryItem);
            }

            var summary = new CreateTaskSummary(list, indexCount, channelsCount, contentsCount, filesCount);

            return summary;
        }
    }
}
