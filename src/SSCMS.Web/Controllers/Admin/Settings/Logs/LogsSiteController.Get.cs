using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SiteLogPageResult>> Get([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
            var adminId = admin?.Id ?? 0;

            var count = await _siteLogRepository.GetCountAsync(request.SiteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo);
            var siteLogs = await _siteLogRepository.GetAllAsync(request.SiteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var siteIdList = await _siteRepository.GetSiteIdsAsync();
            var logTasks = siteLogs.Where(x => siteIdList.Contains(x.SiteId)).Select(async x =>
            {
                var site = await _siteRepository.GetAsync(x.SiteId);
                var log = new SiteLogResult
                {
                    Id = x.Id,
                    SiteId = x.SiteId,
                    ChannelId = x.ChannelId,
                    ContentId = x.ContentId,
                    AdminId = x.AdminId,
                    IpAddress = x.IpAddress,
                    Action = x.Action,
                    Summary = x.Summary,
                    SiteName = site.SiteName,
                    CreatedDate = x.CreatedDate,
                    WebUrl = await _pathManager.GetWebUrlAsync(site)
                };
                return log;
            });
            var logs = await Task.WhenAll(logTasks);

            var siteOptions = await _siteRepository.GetSiteOptionsAsync(0);

            return new SiteLogPageResult
            {
                Items = logs,
                Count = count,
                SiteOptions = siteOptions
            };
        }
    }
}
