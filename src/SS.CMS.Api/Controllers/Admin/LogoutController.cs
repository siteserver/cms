using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers.Admin
{
    [AllowAnonymous]
    [Route("admin")]
    [ApiController]
    public partial class LogoutController : ControllerBase
    {
        public const string Route = "logout";

        private readonly IUserManager _userManager;

        public LogoutController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost(Route)]
        public async Task<ActionResult> Logout()
        {
            await _userManager.SignOutAsync();

            return Ok(new
            {
                Value = true
            });
        }
    }
}