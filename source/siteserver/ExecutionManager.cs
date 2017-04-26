using System;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser;

namespace siteserver
{
    public class ExecutionManager
    {
        public static bool ExecutePendingCreate()
        {
            try
            {
                if (!ServiceManager.IsPendingCreateTask())
                {
                    return false;
                }

                while (true)
                {
                    var taskInfo = CreateTaskManager.Instance.GetLastPendingTask();
                    if (taskInfo == null)
                    {
                        ServiceManager.ClearIsPendingCreateCache();
                        return true;
                    }

                    try
                    {
                        var start = DateTime.Now;
                        var fso = new FileSystemObject(taskInfo.PublishmentSystemID);
                        fso.Execute(taskInfo);
                        var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                        CreateTaskManager.Instance.RemovePendingAndAddSuccessLog(taskInfo, timeSpan);
                    }
                    catch (Exception ex)
                    {
                        CreateTaskManager.Instance.RemovePendingAndAddFailureLog(taskInfo, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddAdminLog(string.Empty, "服务组件生成失败", ex.ToString());
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
