using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home.Write
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ContentsLayerDeleteController : ControllerBase
    {
        private const string Route = "write/contentsLayerDelete";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerDeleteController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Content> Contents { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
        }
    }
}
