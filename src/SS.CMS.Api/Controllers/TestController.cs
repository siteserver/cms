using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers
{
    [Authorize]
    [Route("test")]
    [ApiController]
    public partial class TestController : ControllerBase
    {
        public const string Route = "";

        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;

        public TestController(IUserManager userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet(Route)]
        public IActionResult Test()
        {
            var userName = User.Identity.IsAuthenticated;

            return Ok($"I hope you've got clearance for this {userName}...");
        }
    }
}