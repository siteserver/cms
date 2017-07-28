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
        public void Execute()
        {
            var lockThis = new object();
            lock (lockThis)
            {
                if (ServiceManager.IsServiceOnline())
                {
                    return;
                }
                var taskInfo = CreateTaskManager.Instance.GetLastPendingTask();
                if (taskInfo != null)
                {
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

                    Clients.Client(Context.ConnectionId).next(false);
                }
                else
                {
                    Clients.Client(Context.ConnectionId).next(true);
                }
            }
        }

        public void GetTasks(int publishmentSystemId)
        {
            var lockThis = new object();
            lock (lockThis)
            {
                if (publishmentSystemId > 0)
                {
                    var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                    Clients.Client(Context.ConnectionId)
                        .show(true, summary.Items, summary.IndexCount, summary.ChannelsCount, summary.ContentsCount,
                            summary.FilesCount);
                }
                else
                {
                    if (ServiceManager.IsServiceOnline())
                    {
                        var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                        Clients.Client(Context.ConnectionId).show(true, summary.Items, summary.IndexCount, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);
                    }
                    else
                    {
                        Clients.Client(Context.ConnectionId).show(false, null, 0, 0, 0, 0);
                    }
                }
            }
        }
    }
}