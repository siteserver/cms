using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        /// <summary>
        /// Get User Admin Menus
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/menus
        ///
        /// </remarks>
        /// <response code="200">Returns the menu list</response>
        [ProducesResponseType(200)]
        [Authorize]
        [HttpGet("menus")]
        public async Task<IList<Menu>> GetMenus(string topMenu, int siteId)
        {
            if (StringUtils.EqualsIgnoreCase(topMenu, AuthTypes.Menus.Sites))
            {
                return await GetSiteMenusAsync(siteId);
            }
            return await GetAppMenusAsync(topMenu);
        }
    }
}