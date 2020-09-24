using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerAttributesController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerAttributes";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerAttributesController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsCancel { get; set; }
            public bool IsTop { get; set; }
            public bool IsRecommend { get; set; }
            public bool IsHot { get; set; }
            public bool IsColor { get; set; }
        }
    }
}
