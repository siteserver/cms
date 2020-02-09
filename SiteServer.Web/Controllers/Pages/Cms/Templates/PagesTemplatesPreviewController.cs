using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templatesPreview")]
    public partial class PagesTemplatePreviewController : ApiController
    {
        private const string Route = "";
        private const string RouteCache = "actions/cache";
        private const string CacheKey = "SiteServer.API.Controllers.Pages.Cms.PagesTemplatePreviewController";

        [HttpGet, Route(Route)]
        public async Task<GetResult> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplatePreview);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            var content = CacheUtils.Get<string>(CacheKey);

            return new GetResult
            {
                Channels = cascade,
                Content = content
            };
        }

        [HttpPost, Route(RouteCache)]
        public async Task<BoolResult> Cache([FromBody]CacheRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplatePreview);

            CacheUtils.InsertHours(CacheKey, request.Content, 1);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<StringResult> Submit([FromBody]SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplatePreview);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<StringResult>();

            var contentId = 0;
            if (request.TemplateType == TemplateType.ContentTemplate)
            {
                var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channelInfo);
                if (count > 0)
                {
                    var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelInfo);
                    contentId = await DataProvider.ContentRepository.GetFirstContentIdAsync(tableName, request.ChannelId);
                }

                if (contentId == 0)
                {
                    return Request.BadRequest<StringResult>("所选栏目下无内容，请选择有内容的栏目");
                }
            }

            CacheUtils.InsertHours(CacheKey, request.Content, 1);

            var parsedContent = await StlParserManager.ParseTemplatePreviewAsync(site, request.TemplateType, request.ChannelId, contentId, request.Content);

            return new StringResult
            {
                Value = parsedContent
            };
        }
    }
}
