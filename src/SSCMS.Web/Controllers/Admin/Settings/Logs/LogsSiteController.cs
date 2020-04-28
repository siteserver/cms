using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsSiteController : ControllerBase
    {
        private const string Route = "settings/logsSite";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteLogRepository _siteLogRepository;

        public LogsSiteController(IAuthManager authManager, IPathManager pathManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, ISiteLogRepository siteLogRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _siteLogRepository = siteLogRepository;
        }

        //[HttpGet, Route(Route)]
        //public async Task<SiteLogPageResult> List([FromBody] PageRequest request)
        //{
        //    
        //    await _authManager.CheckPermissionAsync(Request, AuthTypes.AppPermissions.SettingsLog);

        //    var count = await _siteLogRepository.GetCountAsync(null, null, null, null, null, null);
        //    var siteLogs = await _siteLogRepository.GetAllAsync(null, null, null, null, null, null, request.Offset, request.Limit);
        //    var siteOptions = await _siteRepository.GetSiteOptionsAsync(0);

        //    var siteIdList = await _siteRepository.GetSiteIdListAsync();
        //    var logTasks = siteLogs.Where(x => siteIdList.Contains(x.SiteId)).Select(async x =>
        //    {
        //        var site = await _siteRepository.GetAsync(x.SiteId);
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
            var adminId = admin?.Id ?? 0;

            var count = await _siteLogRepository.GetCountAsync(request.SiteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo);
            var siteLogs = await _siteLogRepository.GetAllAsync(request.SiteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var siteIdList = await _siteRepository.GetSiteIdListAsync();
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

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            await _siteLogRepository.DeleteAllAsync();

            await _authManager.AddAdminLogAsync("清空站点日志");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
