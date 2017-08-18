using System;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser;

namespace siteserver
{
    public class ExecutionManager
    {
        public static void ClearAllPendingCreate()
        {
            CreateTaskManager.Instance.ClearAllTask();
        }

        public static bool ExecutePendingCreate()
        {
            try
            {
                if (!ServiceManager.IsPendingCreateTask()) return false;

                var instance = CreateTaskManager.Instance;

                while (true)
                {
                    var taskInfo = instance.GetAndRemoveLastPendingTask(0);
                    if (taskInfo == null)
                    {
                        ServiceManager.ClearIsPendingCreateCache();
                        return true;
                    }

                    try
                    {
                        var start = DateTime.Now;
                        FileSystemObject.Execute(taskInfo.PublishmentSystemId, taskInfo.CreateType, taskInfo.ChannelId,
                            taskInfo.ContentId, taskInfo.TemplateId);
                        var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                        instance.AddSuccessLog(taskInfo, timeSpan);
                    }
                    catch (Exception ex)
                    {
                        instance.AddFailureLog(taskInfo, ex);
                    }
                    finally
                    {
                        instance.RemoveCurrent(taskInfo.PublishmentSystemId, taskInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, "服务组件生成失败");
            }

            return false;
        } 

        public static bool ExecuteTask()
        {
            var isExecute = false;
            try
            {
                var taskInfoList = ServiceManager.GetAllTaskInfoList();
                foreach (var taskInfo in taskInfoList)
                {
                    if (!ExecutionUtils.IsNeedExecute(taskInfo))
                    {
                        continue;
                    }

                    try
                    {
                        var taskExecution = new TaskExecution();

                        if (taskExecution.Execute(taskInfo))
                        {
                            isExecute = true;
                            var logInfo = new TaskLogInfo(0, taskInfo.TaskID, true, string.Empty, DateTime.Now);
                            DataProvider.TaskLogDao.Insert(logInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExecutionUtils.LogError(taskInfo, ex);
                    }

                    DataProvider.TaskDao.UpdateLastExecuteDate(taskInfo.TaskID);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddAdminLog(string.Empty, "服务组件任务执行失败", ex.ToString());
            }

            return isExecute;
        }
    }
}
