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
        public async Task Execute(int siteId)
        {
            var pendingTaskCount = 0;
            try
            {
                pendingTaskCount = await ExecuteTaskAsync(siteId);
            }
            catch (Exception ex)
            {
                LogUtils.AddSystemErrorLog(ex, "CreateHub");
            }

            Clients.Client(Context.ConnectionId).next(pendingTaskCount);
        }

        private static async Task<int> ExecuteTaskAsync(int siteId)
        {
            var instance = CreateTaskManager.Instance;

            var pendingTask = instance.GetAndRemoveLastPendingTask(siteId);
            if (pendingTask == null) return 0;

            try
            {
                var start = DateTime.Now;
                await FileSystemObjectAsync.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType, pendingTask.ChannelId,
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
                instance.RemoveCurrent(siteId, pendingTask);
            }

            return instance.GetPendingTaskCount(siteId);
        }

        public async Task GetTasks(int siteId)
        {
            try
            {
                if (siteId > 0)
                {
                    var summary = CreateTaskManager.Instance.GetTaskSummary(siteId);
                    Clients.Client(Context.ConnectionId).show(true, summary.Tasks, summary.ChannelsCount, summary.ContentsCount, summary.FilesCount);

                    await Execute(siteId);
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