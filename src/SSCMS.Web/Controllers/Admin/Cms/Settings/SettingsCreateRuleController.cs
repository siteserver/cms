using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsCreateRuleController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCreateRule";
        private const string RouteGet = "cms/settings/settingsCreateRule/{siteId:int}/{channelId:int}";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsCreateRuleController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channel { get; set; }
        }

        public class ChannelResult
        {
            public Channel Channel { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string LinkUrl { get; set; }
            public LinkType LinkType { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
        }
    }
}