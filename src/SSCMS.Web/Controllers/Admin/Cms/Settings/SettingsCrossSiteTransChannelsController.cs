using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsCrossSiteTransChannelsController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCrossSiteTransChannels";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITranslateRepository _translateRepository;

        public SettingsCrossSiteTransChannelsController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITranslateRepository translateRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _translateRepository = translateRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public List<Translate> Translates { get; set; }
        }
    }
}