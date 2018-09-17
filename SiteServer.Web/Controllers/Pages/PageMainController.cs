using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("api/pages/main")]
    public class PageMainController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = new AuthRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var count = CreateTaskManager.PendingTaskCount;

                var pendingTask = CreateTaskManager.GetFirstPendingTask();
                if (pendingTask != null)
                {
                    try
                    {
                        var start = DateTime.Now;
                        await FileSystemObjectAsync.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType,
                            pendingTask.ChannelId,
                            pendingTask.ContentId, pendingTask.FileTemplateId, pendingTask.SpecialId);
                        var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                        CreateTaskManager.AddSuccessLog(pendingTask, timeSpan);
                    }
                    catch (Exception ex)
                    {
                        CreateTaskManager.AddFailureLog(pendingTask, ex);
                    }
                    finally
                    {
                        CreateTaskManager.RemovePendingTask(pendingTask);
                    }
                }

                return Ok(new
                {
                    Value = count
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}