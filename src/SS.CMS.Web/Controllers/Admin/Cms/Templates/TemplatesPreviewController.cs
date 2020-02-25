using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.StlParser.Utility;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesPreview")]
    public partial class TemplatesPreviewController : ControllerBase
    {
        private const string Route = "";
        private const string RouteCache = "actions/cache";
        private const string CacheKey = "SiteServer.API.Controllers.Pages.Cms.PagesTemplatePreviewController";

        private readonly IAuthManager _authManager;

        public TemplatesPreviewController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplatePreview))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

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
        public async Task<ActionResult<BoolResult>> Cache([FromBody]CacheRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplatePreview))
            {
                return Unauthorized();
            }

            CacheUtils.InsertHours(CacheKey, request.Content, 1);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplatePreview))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

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
                    return this.Error("所选栏目下无内容，请选择有内容的栏目");
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
