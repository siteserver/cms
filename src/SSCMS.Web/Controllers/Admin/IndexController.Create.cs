using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [HttpPost, Route(RouteActionsCreate)]
        public async Task<ActionResult<IntResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.IsAdminAuthenticatedAsync())
            {
                return Unauthorized();
            }

            var admin = await _authManager.GetAdminAsync();
            var cacheKey = Constants.GetSessionIdCacheKey(admin.Id);
            var sessionId = await _dbCacheRepository.GetValueAsync(cacheKey);
            if (string.IsNullOrEmpty(request.SessionId) || sessionId != request.SessionId)
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            if (admin.LastActivityDate != null && config.IsAdminEnforceLogout)
            {
                var ts = new TimeSpan(DateTime.Now.Ticks - admin.LastActivityDate.Value.Ticks);
                if (ts.TotalMinutes > config.AdminEnforceLogoutMinutes)
                {
                    return Unauthorized();
                }
            }

            var count = _createManager.PendingTaskCount;

            var pendingTask = _createManager.GetFirstPendingTask();
            if (pendingTask != null)
            {
                try
                {
                    var start = DateTime.Now;
                    await _createManager.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType,
                        pendingTask.ChannelId,
                        pendingTask.ContentId, pendingTask.FileTemplateId, pendingTask.SpecialId);
                    var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                    _createManager.AddSuccessLog(pendingTask, timeSpan);
                }
                catch (Exception ex)
                {
                    _createManager.AddFailureLog(pendingTask, ex);
                }
                finally
                {
                    _createManager.RemovePendingTask(pendingTask);
                }
            }

            return new IntResult
            {
                Value = count
            };
        }
    }
}
