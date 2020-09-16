using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class ChannelsController : ControllerBase
    {
        private const string RouteSite = "channels/{siteId:int}";
        private const string RouteChannel = "channels/{siteId:int}/{channelId:int}";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ChannelsController(IAuthManager authManager, ICreateManager createManager, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _accessTokenRepository = accessTokenRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class CreateRequest : Dictionary<string, object>
        {
            public int ParentId { get; set; }
            public string ContentModelPluginId { get; set; }
            public List<string> ContentRelatedPluginIds { get; set; }
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public List<string> GroupNames { get; set; }
            public string ImageUrl { get; set; }
            public string Content { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
            public string LinkUrl { get; set; }
            public LinkType LinkType { get; set; }
            public int ChannelTemplateId { get; set; }
            public int ContentTemplateId { get; set; }
        }

        public class UpdateRequest : Dictionary<string, object>
        {
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
            public string ContentModelPluginId { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public List<string> GroupNames { get; set; }
            public string ImageUrl { get; set; }
            public string Content { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
            public string LinkUrl { get; set; }
            public string LinkType { get; set; }
            public int? ChannelTemplateId { get; set; }
            public int? ContentTemplateId { get; set; }
        }
    }
}
