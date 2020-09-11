using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigController : ControllerBase
    {
        private const string Route = "plugins/config";
        private const string RouteActionsGetChannels = "plugins/config/actions/getChannels";
        private const string RouteActionsSubmitChannels = "plugins/config/actions/submitChannels";
        private const string RouteActionsRestart = "plugins/config/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ConfigController(IHostApplicationLifetime hostApplicationLifetime, IAuthManager authManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _authManager = authManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class GetRequest
        {
            public string PluginId { get; set; }
        }

        public class GetResult
        {
            public IPlugin Plugin { get; set; }
            public List<Cascade<int>> Sites { get; set; }
        }

        public class GetChannelsRequest : SiteRequest
        {
            public string PluginId { get; set; }
        }

        public class GetChannelsResult
        {
            public string SiteName { get; set; }
            public Channel Channel { get; set; }
            public SiteConfig SiteConfig { get; set; }
        }

        public class SubmitChannelsRequest
        {
            public string PluginId { get; set; }
            public int SiteId { get; set; }
            public bool AllChannels { get; set; }
            public List<int> ChannelIds { get; set; }
        }

        public class SubmitRequest
        {
            public string PluginId { get; set; }
            public int Taxis { get; set; }
            public bool AllSites { get; set; }
            public List<int> SiteIds { get; set; }
        }
    }
}
