using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

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

        private readonly ICacheManager _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public TemplatesPreviewController(ICacheManager cacheManager, IAuthManager authManager, IParseManager parseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public string Content { get; set; }
        }

        public class CacheRequest
        {
            public int SiteId { get; set; }
            public string Content { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public TemplateType TemplateType { get; set; }
            public int ChannelId { get; set; }
            public string Content { get; set; }
        }
    }
}
