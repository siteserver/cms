using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Web.Controllers
{
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public HealthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<User>> Get()
        {
            var user = await _authManager.GetUserAsync();
            return user;
        }
    }
}