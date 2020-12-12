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
    public partial class ContentsLayerViewController : ControllerBase
    {
        private const string Route = "write/contentsLayerView";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerViewController(IAuthManager authManager, IDatabaseManager databaseManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { set; get; }
        }

        public class GetResult
        {
            public Content Content { set; get; }
            public string ChannelName { set; get; }
            public List<ContentColumn> Attributes { set; get; }
        }
    }
}
