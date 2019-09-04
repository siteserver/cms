using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;

namespace SS.CMS.Api.Controllers.Sites
{
    public partial class SitesController
    {
        /// <summary>
        /// List Sites
        /// </summary>
        /// <response code="200">Returns the sites list</response>
        /// <response code="401">Unauthorized</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<IList<Site>>> List()
        {
            var list = await _siteRepository.GetSiteListAsync(0);

            return list.ToList();
        }
    }
}
