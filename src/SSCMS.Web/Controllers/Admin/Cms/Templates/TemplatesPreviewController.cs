using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesPreviewController : ControllerBase
    {
        private const string Route = "cms/templates/templatesPreview";
        private const string RouteCache = "cms/templates/templatesPreview/actions/cache";

        private static readonly string CacheKey = CacheUtils.GetClassKey(typeof(TemplatesPreviewController));

        private readonly ICacheManager<string> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public TemplatesPreviewController(ICacheManager<string> cacheManager, IAuthManager authManager, IParseManager parseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesPreview))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            var content = _cacheManager.Get(CacheKey);

            return new GetResult
            {
                Channels = cascade,
                Content = content
            };
        }

        [HttpPost, Route(RouteCache)]
        public async Task<ActionResult<BoolResult>> Cache([FromBody]CacheRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesPreview))
            {
                return Unauthorized();
            }

            _cacheManager.AddOrUpdateSliding(CacheKey, request.Content, 60);

            //var cacheItem = new CacheItem<string>(CacheKey, request.Content, ExpirationMode.Sliding, TimeSpan.FromHours(1));
            //_cacheManager.AddOrUpdate(cacheItem, _ => request.Content);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesPreview))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var contentId = 0;
            if (request.TemplateType == TemplateType.ContentTemplate)
            {
                var channel = await _channelRepository.GetAsync(request.ChannelId);
                var count = await _contentRepository.GetCountAsync(site, channel);
                if (count > 0)
                {
                    var tableName = _channelRepository.GetTableName(site, channel);
                    contentId = await _contentRepository.GetFirstContentIdAsync(tableName, request.ChannelId);
                }

                if (contentId == 0)
                {
                    return this.Error("所选栏目下无内容，请选择有内容的栏目");
                }
            }

            _cacheManager.AddOrUpdateSliding(CacheKey, request.Content, 60);

            //var cacheItem = new CacheItem<string>(CacheKey, request.Content, ExpirationMode.Sliding, TimeSpan.FromHours(1));
            //_cacheManager.AddOrUpdate(cacheItem, _ => request.Content);

            var templateInfo = new Template
            {
                TemplateType = request.TemplateType
            };

            await _parseManager.InitAsync(site, request.ChannelId, contentId, templateInfo);

            var parsedContent = await _parseManager.ParseTemplatePreviewAsync(request.Content);

            return new StringResult
            {
                Value = parsedContent
            };
        }
    }
}
