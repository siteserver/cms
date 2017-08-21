using System;
using BaiRong.Core;
using Microsoft.AspNet.SignalR;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.StlParser;

namespace SiteServer.API
{
    public class CreateHub : Hub
    {
        public void Execute(int publishmentSystemId)
        {
            var pendingTaskCount = 0;
            try
            {
                pendingTaskCount = ExecuteTask(publishmentSystemId);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, "CreateHub");
            }

            Clients.Client(Context.ConnectionId).next(pendingTaskCount);
        }

        private static int ExecuteTask(int publishmentSystemId)
        {
            // 如果服务组件启用了的话，则通过服务组件生成
            if (ServiceManager.IsServiceOnline)
            {
                return 0;
            }

            var instance = CreateTaskManager.Instance;

            var pendingTask = instance.GetAndRemoveLastPendingTask(publishmentSystemId);
            if (pendingTask == null) return 0;

            try
            {
                var start = DateTime.Now;
                FileSystemObject.Execute(pendingTask.PublishmentSystemId, pendingTask.CreateType, pendingTask.ChannelId,
                    pendingTask.ContentId, pendingTask.TemplateId);
                var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                instance.AddSuccessLog(pendingTask, timeSpan);
            }
            catch (Exception ex)
            {
                instance.AddFailureLog(pendingTask, ex);
            }
            finally
            {
                instance.RemoveCurrent(publishmentSystemId, pendingTask);
            }

            return instance.GetPendingTaskCount(publishmentSystemId);
        }

        public void GetTasks(int publishmentSystemId)
        {
            try
            {
                if (publishmentSystemId > 0)
                {
                    var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                    Clients.Client(Context.ConnectionId).show(true, summary.Tasks, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);

                    Execute(publishmentSystemId);
                }
                else
                {
                    if (ServiceManager.IsServiceOnline)
                    {
                        var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                        Clients.Client(Context.ConnectionId).show(true, summary.Tasks, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);
                    }
                    else
                    {
                        Clients.Client(Context.ConnectionId).show(false, null, 0, 0, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, "CreateHub");
            }
        }
    }
}