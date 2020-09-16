using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ChannelsLayerGroupController : ControllerBase
    {
        private const string Route = "cms/channels/channelsLayerGroup";
        private const string RouteAdd = "cms/channels/channelsLayerGroup/actions/add";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;

        public ChannelsLayerGroupController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IChannelGroupRepository channelGroupRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _channelGroupRepository = channelGroupRepository;
        }

        public class SubmitRequest : ChannelRequest
        {
            public List<int> ChannelIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
        }

        public class AddRequest : ChannelRequest
        {
            public List<int> ChannelIds { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }
    }
}
