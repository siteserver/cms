using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;

namespace SS.CMS.Api.Controllers.Sites
{
    public partial class SitesController
    {
        /// <summary>
        /// Get Site
        /// </summary>
        /// <response code="200">Returns the site info</response>
        /// <response code="401">Unauthorized</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Site>> GetById(int id)
        {
            var site = await _siteRepository.GetSiteAsync(id);

            return site;
        }
    }
}
