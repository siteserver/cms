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

        private int ExecuteTask(int publishmentSystemId)
        {
            // 如果服务组件启用了的话，则通过服务组件生成
            if (ServiceManager.IsServiceOnline())
            {
                return 0;
            }

            var pendingTask = CreateTaskManager.Instance.GetAndRemoveLastPendingTask(publishmentSystemId);
            if (pendingTask == null) return 0;

            try
            {
                var start = DateTime.Now;
                var fso = new FileSystemObject(pendingTask.PublishmentSystemId);
                fso.Execute(pendingTask.CreateType, pendingTask.ChannelId, pendingTask.ContentId, pendingTask.TemplateId);
                var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                CreateTaskManager.Instance.AddSuccessLog(pendingTask, timeSpan);
            }
            catch (Exception ex)
            {
                CreateTaskManager.Instance.AddFailureLog(pendingTask, ex);
            }

            //CreateTaskManager.Instance.RemoveTask(publishmentSystemId, pendingTask);

            return CreateTaskManager.Instance.GetPendingTaskCount(publishmentSystemId);
        }

        public void GetTasks(int publishmentSystemId)
        {
            try
            {
                if (publishmentSystemId > 0)
                {
                    var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                    Clients.Client(Context.ConnectionId).show(true, summary.Current, summary.Tasks, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);

                    Execute(publishmentSystemId);
                }
                else
                {
                    if (ServiceManager.IsServiceOnline())
                    {
                        var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                        Clients.Client(Context.ConnectionId).show(true, summary.Current, summary.Tasks, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);
                    }
                    else
                    {
                        Clients.Client(Context.ConnectionId).show(false, null, null, 0, 0, 0, 0);
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