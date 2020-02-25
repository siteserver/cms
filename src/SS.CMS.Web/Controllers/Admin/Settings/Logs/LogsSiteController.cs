using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    [Route("admin/settings/logsSite")]
    public partial class LogsSiteController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public LogsSiteController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        //[HttpGet, Route(Route)]
        //public async Task<SiteLogPageResult> List([FromBody] PageRequest request)
        //{
        //    var auth = await _authManager.GetAdminAsync();
        //    await auth.CheckPermissionAsync(Request, Constants.AppPermissions.SettingsLog);

        //    var count = await DataProvider.SiteLogRepository.GetCountAsync(null, null, null, null, null, null);
        //    var siteLogs = await DataProvider.SiteLogRepository.GetAllAsync(null, null, null, null, null, null, request.Offset, request.Limit);
        //    var siteOptions = await DataProvider.SiteRepository.GetSiteOptionsAsync(0);

        //    var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync();
        //    var logTasks = siteLogs.Where(x => siteIdList.Contains(x.SiteId)).Select(async x =>
        //    {
        //        var site = await DataProvider.SiteRepository.GetAsync(x.SiteId);
        //        var log = new SiteLogResult
        //        {
        //            Id = x.Id,
        //            SiteId = x.SiteId,
        //            ChannelId = x.ChannelId,
        //            ContentId = x.ContentId,
        //            UserName = x.UserName,
        //            IpAddress = x.IpAddress,
        //            AddDate = x.AddDate,
        //            Action = x.Action,
        //            Summary = x.Summary,
        //            SiteName = site.SiteName,
        //            WebUrl = site.WebUrl
        //        };
        //        return log;
        //    });
        //    var logs = await Task.WhenAll(logTasks);

        //    return new SiteLogPageResult
        //    {
        //        Items = logs,
        //        Count = count,
        //        SiteOptions = siteOptions
        //    };
        //}

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SiteLogPageResult>> List([FromBody] SearchRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            var admin = await DataProvider.AdministratorRepository.GetByUserNameAsync(request.UserName);
            var adminId = admin?.Id ?? 0;

            var count = await DataProvider.SiteLogRepository.GetCountAsync(request.SiteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo);
            var siteLogs = await DataProvider.SiteLogRepository.GetAllAsync(request.SiteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync();
            var logTasks = siteLogs.Where(x => siteIdList.Contains(x.SiteId)).Select(async x =>
            {
                var site = await DataProvider.SiteRepository.GetAsync(x.SiteId);
                var log = new SiteLogResult
                {
                    Id = x.Id,
                    SiteId = x.SiteId,
                    ChannelId = x.ChannelId,
                    ContentId = x.ContentId,
                    AdminId = x.AdminId,
                    IpAddress = x.IpAddress,
                    AddDate = x.AddDate,
                    Action = x.Action,
                    Summary = x.Summary,
                    SiteName = site.SiteName,
                    WebUrl = await site.GetWebUrlAsync()
                };
                return log;
            });
            var logs = await Task.WhenAll(logTasks);

            var siteOptions = await DataProvider.SiteRepository.GetSiteOptionsAsync(0);

            return new SiteLogPageResult
            {
                Items = logs,
                Count = count,
                SiteOptions = siteOptions
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            await DataProvider.SiteLogRepository.DeleteAllAsync();

            await auth.AddAdminLogAsync("清空站点日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
