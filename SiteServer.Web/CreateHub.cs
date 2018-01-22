using System;
using System.Threading.Tasks;
using SiteServer.Utils;
using Microsoft.AspNet.SignalR;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.StlParser;

namespace SiteServer.API
{
    public class CreateHub : Hub
    {
        public async Task Execute(int publishmentSystemId)
        {
            var pendingTaskCount = 0;
            try
            {
                pendingTaskCount = await ExecuteTaskAsync(publishmentSystemId);
            }
            catch (Exception ex)
            {
                LogUtils.AddSystemErrorLog(ex, "CreateHub");
            }

            Clients.Client(Context.ConnectionId).next(pendingTaskCount);
        }

        private static async Task<int> ExecuteTaskAsync(int publishmentSystemId)
        {
            var instance = CreateTaskManager.Instance;

            var pendingTask = instance.GetAndRemoveLastPendingTask(publishmentSystemId);
            if (pendingTask == null) return 0;

            try
            {
                var start = DateTime.Now;
                await FileSystemObjectAsync.ExecuteAsync(pendingTask.PublishmentSystemId, pendingTask.CreateType, pendingTask.ChannelId,
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

        public async Task GetTasks(int publishmentSystemId)
        {
            try
            {
                if (publishmentSystemId > 0)
                {
                    var summary = CreateTaskManager.Instance.GetTaskSummary(publishmentSystemId);
                    Clients.Client(Context.ConnectionId).show(true, summary.Tasks, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);

                    await Execute(publishmentSystemId);
                }
                else
                {
                    Clients.Client(Context.ConnectionId).show(false, null, 0, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddSystemErrorLog(ex, "CreateHub");
            }
        }
    }
}