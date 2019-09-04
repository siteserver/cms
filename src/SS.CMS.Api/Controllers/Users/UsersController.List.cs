using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        /// <summary>
        /// List Users
        /// </summary>
        /// <response code="200">Returns the users list</response>
        /// <response code="401">Unauthorized</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<IList<User>>> List()
        {
            var list = await _userRepository.GetAllAsync(null);

            return list.ToList();
        }
    }
}
