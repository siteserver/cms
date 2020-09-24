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
    public partial class ContentsLayerGroupController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerGroup";
        private const string RouteAdd = "cms/contents/contentsLayerGroup/actions/add";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;

        public ContentsLayerGroupController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
        }

        public class AddRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }
    }
}
