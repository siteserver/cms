using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Users
{
    [ApiController]
    [AllowAnonymous]
    [Route("users")]
    public partial class UsersController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;

        public UsersController(ISettingsManager settingsManager, IUserManager userManager, IUserRepository userRepository)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
            _userRepository = userRepository;
        }
    }
}