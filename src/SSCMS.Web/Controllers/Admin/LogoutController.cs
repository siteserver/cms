using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [Route(Constants.ApiRoute)]
    public partial class LogoutController : ControllerBase
    {
        public const string Route = "logout";

        private readonly IAuthManager _authManager;

        public LogoutController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<BoolResult> Logout()
        {
            _authManager.AdminLogout();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}