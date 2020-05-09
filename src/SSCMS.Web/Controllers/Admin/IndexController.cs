using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SSCMS.Utils;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteActionsSetLanguage = "index/actions/setLanguage";
        private const string RouteActionsCreate = "index/actions/create";
        private const string RouteActionsCache = "index/actions/cache";
        private const string RouteActionsDownload = "index/actions/download";

        private readonly IStringLocalizer<IndexController> _local;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public IndexController(IStringLocalizer<IndexController> local, ISettingsManager settingsManager, IAuthManager authManager, ICreateManager createManager, IPathManager pathManager, IPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDbCacheRepository dbCacheRepository)
        {
            _local = local;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _dbCacheRepository = dbCacheRepository;
        }
    }
}