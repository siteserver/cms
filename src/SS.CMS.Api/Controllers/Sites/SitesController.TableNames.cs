using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SS.CMS.Api.Controllers.Sites
{
    public partial class SitesController
    {
        /// <summary>
        /// /sites/tableNames
        /// </summary>
        /// <remarks>
        /// List Content Table Names
        /// </remarks>
        /// <response code="200">Returns the table names list</response>
        /// <response code="401">Unauthorized</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [HttpGet("tableNames")]
        public async Task<ActionResult<IList<string>>> GetTableNames()
        {
            var tableNames = await _siteRepository.GetSiteTableNamesAsync(_pluginManager);

            return tableNames;
        }
    }
}
