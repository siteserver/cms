using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Sites
{
    [ApiController]
    [AllowAnonymous]
    [Route("sites")]
    public partial class SitesController : ControllerBase
    {
        public const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;

        public SitesController(ISettingsManager settingsManager, IUserManager userManager, IUserRepository userRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
        }

        [Authorize]
        [HttpGet(Route)]
        public async Task<ActionResult<IList<SiteInfo>>> List()
        {
            var siteIdList = await _siteRepository.GetSiteIdListOrderByLevelAsync();
            var list = new List<SiteInfo>();
            foreach (var siteId in siteIdList)
            {
                list.Add(await _siteRepository.GetSiteInfoAsync(siteId));
            }

            return list;
        }
    }
}