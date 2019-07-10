using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Themes
{
    [ApiController]
    [AllowAnonymous]
    [Route("themes")]
    public partial class ThemesController : ControllerBase
    {
        public const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;

        public ThemesController(ISettingsManager settingsManager, IPathManager pathManager, IUserManager userManager, IUserRepository userRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
        }

        [Authorize]
        [HttpGet(Route)]
        public ActionResult<IList<Package>> List()
        {
            var themeManager = new ThemeManager(_pathManager);

            return themeManager.GetThemeInfoList();
        }
    }
}