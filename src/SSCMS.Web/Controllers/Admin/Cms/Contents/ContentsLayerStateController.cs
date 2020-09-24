using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerStateController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerState";

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
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public List<ContentCheck> ContentChecks { get; set; }
            public Content Content { get; set; }
            public string State { get; set; }
        }
    }
}
