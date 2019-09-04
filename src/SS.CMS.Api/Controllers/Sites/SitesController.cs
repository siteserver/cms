using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Data;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Sites
{
    [ApiController]
    [AllowAnonymous]
    [Route("sites")]
    public partial class SitesController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabase _database;
        private readonly IPluginManager _pluginManager;
        private readonly IUserManager _userManager;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public SitesController(ISettingsManager settingsManager, IDatabase database, IPluginManager pluginManager, IUserManager userManager, IDatabaseRepository databaseRepository, IUserRepository userRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _database = database;
            _pluginManager = pluginManager;
            _userManager = userManager;
            _databaseRepository = databaseRepository;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }
    }
}