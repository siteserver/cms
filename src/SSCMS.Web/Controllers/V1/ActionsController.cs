using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class ActionsController : ControllerBase
    {
        private const string RouteCreate = "actions/create";
        private const string RouteClearCache = "actions/clearCache";
        private const string RouteRestart = "actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAuthManager _authManager;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly ICreateManager _createManager;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISpecialRepository _specialRepository;

        public ActionsController(IHostApplicationLifetime hostApplicationLifetime, IAuthManager authManager, ISettingsManager settingsManager, ICacheManager cacheManager, ICreateManager createManager, IDbCacheRepository dbCacheRepository, IAccessTokenRepository accessTokenRepository, ITemplateRepository templateRepository, ISpecialRepository specialRepository)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _authManager = authManager;
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
            _createManager = createManager;
            _dbCacheRepository = dbCacheRepository;
            _accessTokenRepository = accessTokenRepository;
            _templateRepository = templateRepository;
            _specialRepository = specialRepository;
        }

        public class ChannelContentId
        {
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public CreateType Type { get; set; }
            public List<int> ChannelIds { get; set; }
            public List<ChannelContentId> ChannelContentIds { get; set; }
            public string Name { get; set; }
        }
    }
}
