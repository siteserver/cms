using System.Collections.Generic;
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
    public partial class ContentsLayerTagController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerTag";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentTagRepository _contentTagRepository;

        public ContentsLayerTagController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentTagRepository contentTagRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentTagRepository = contentTagRepository;
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> TagNames { get; set; }
        }
    }
}
