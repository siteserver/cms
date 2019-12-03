using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates")]
    public partial class PagesTemplatesController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<SiteLogPageResult> List([FromBody] SearchRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.WebSitePermissions.Template);

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync();
            var count = await DataProvider.SiteLogRepository.GetCountAsync(siteIdList, request.LogType, request.UserName, request.Keyword, request.DateFrom, request.DateTo);
            var siteLogs = await DataProvider.SiteLogRepository.GetAllAsync(siteIdList, request.LogType, request.UserName, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var logTasks = siteLogs.Where(x => siteIdList.Contains(x.SiteId)).Select(async x =>
            {
                var site = await DataProvider.SiteRepository.GetAsync(x.SiteId);
                var log = new SiteLogResult
                {
                    Id = x.Id,
                    SiteId = x.SiteId,
                    ChannelId = x.ChannelId,
                    ContentId = x.ContentId,
                    UserName = x.UserName,
                    IpAddress = x.IpAddress,
                    AddDate = x.AddDate,
                    Action = x.Action,
                    Summary = x.Summary,
                    SiteName = site.SiteName,
                    WebUrl = site.GetWebUrl()
                };
                return log;
            });
            var logs = await Task.WhenAll(logTasks);

            var dictionary = await DataProvider.TemplateRepository.GetCountDictionaryAsync(request.SiteId);
            dictionary.TryGetValue(TemplateType.IndexPageTemplate, out var indexPageTemplateCount);
            dictionary.TryGetValue(TemplateType.ChannelTemplate, out var channelTemplateCount);
            dictionary.TryGetValue(TemplateType.ContentTemplate, out var contentTemplateCount);
            dictionary.TryGetValue(TemplateType.FileTemplate, out var fileTemplateCount);

            var result = new SiteLogPageResult
            {
                Items = logs,
                Count = count,
                IndexPageTemplateCount = indexPageTemplateCount,
                ChannelTemplateCount = channelTemplateCount,
                ContentTemplateCount = contentTemplateCount,
                FileTemplateCount = fileTemplateCount
            };

            return result;
        }

        [HttpDelete, Route(Route)]
        public async Task<DefaultResult> Delete()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSettingsPermissions(Request, Constants.SettingsPermissions.Log);

            await DataProvider.SiteLogRepository.DeleteAllAsync();

            await auth.AddAdminLogAsync("清空站点日志");

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
