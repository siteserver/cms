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
    public partial class ContentsLayerStateController : ControllerBase
    {
        private const string Route = "write/contentsLayerState";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public ContentsLayerStateController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentCheckRepository contentCheckRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentCheckRepository = contentCheckRepository;
            _administratorRepository = administratorRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { set; get; }
        }

        public class GetResult
        {
            public List<ContentCheck> ContentChecks { get; set; }
            public Content Content { get; set; }
            public string State { get; set; }
        }
    }
}
