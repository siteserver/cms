using Microsoft.AspNetCore.Mvc;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        /// <summary>
        /// User Logout
        /// </summary>
        /// <response code="200"></response>
        [ProducesResponseType(200)]
        [HttpPost("logout")]
        public ActionResult<LogoutResponse> Logout()
        {
            //await _userManager.SignOutAsync();

            return new LogoutResponse
            {
                Value = true
            };
        }

        public class LogoutResponse
        {
            public bool Value{get;set;}
        }
    }
}