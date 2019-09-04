using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        /// <summary>
        /// Get Current User Info
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/info
        ///
        /// </remarks>
        /// <returns>User info</returns>
        /// <response code="200">Returns the current user info</response>
        /// <response code="404">If current user is not exists</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize]
        [HttpGet("info")]
        public async Task<ActionResult<InfoResponse>> GetInfo()
        {
            var userInfo = await _userManager.GetUserAsync();
            if (userInfo == null || userInfo.IsLockedOut) return NotFound();
            var menus = await GetTopMenusAsync();

            return new InfoResponse
            {
                Id = userInfo.Id,
                DisplayName = userInfo.DisplayName,
                UserName = userInfo.UserName,
                AvatarUrl = userInfo.AvatarUrl,
                Bio = userInfo.Bio,
                Roles = new string[] { "admin" },
                Menus = menus
            };
        }

        public class InfoResponse
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string AvatarUrl { get; set; }
            public string Bio { get; set; }
            public IList<string> Roles { get; set; }
            public IList<Menu> Menus { get; set; }
        }
    }
}