using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Models;

namespace SS.CMS.Api.Controllers.Sites
{
    public partial class SitesController
    {
        /// <summary>
        /// Delete Site
        /// </summary>
        /// <response code="200">Returns the deleted site info</response>
        /// <response code="401">Unauthorized</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [ClaimRequirement(AuthTypes.ClaimTypes.Role, AuthTypes.Roles.SuperAdministrator)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Site>> DeleteById(int id)
        {
            if (!_userManager.IsSuperAdministrator())
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetSiteAsync(id);
            if (await _siteRepository.DeleteAsync(id))
            {
                return site;
            }
            return null;
        }
    }
}
