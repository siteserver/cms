using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Cms
{
    [ApiController]
    [AllowAnonymous]
    [Route("cms")]
    public partial class CmsController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;
        private readonly IConfigRepository _configRepository;

        public CmsController(IDistributedCache cache, ISettingsManager settingsManager, IUserManager userManager, IConfigRepository configRepository)
        {
            _cache = cache;
            _settingsManager = settingsManager;
            _userManager = userManager;
            _configRepository = configRepository;
        }
    }
}