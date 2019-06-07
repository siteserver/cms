using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.Admin;
using SS.CMS.Core.Services.Admin.Settings.Admin;

namespace SS.CMS.Api.Controllers.Admin.Settings.Admin
{
    [Route(AdminRoutes.PrefixSettingsAdmin)]
    [ApiController]
    public class AccessTokenController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly AccessTokenService _service;

        public AccessTokenController(Request request, Response response, AccessTokenService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        /// <summary>
        /// List Access Tokens.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     GET /api/admin/settings/admin/access-token
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="401">If admin is not authorized</response>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200)]     // Ok
        [ProducesResponseType(400)]     // BadRequest
        [ProducesResponseType(401)]     // Unauthorized
        [HttpGet(AccessTokenService.Route)]
        public async Task<ActionResult> List()
        {
            return await _service.RunAsync(_request, _response, _service.ListAsync);
        }

        [HttpPost(AccessTokenService.Route)]
        public async Task<ActionResult> Create()
        {
            return await _service.RunAsync(_request, _response, _service.CreateAsync);
        }

        [HttpPut(AccessTokenService.Route)]
        public async Task<ActionResult> Update()
        {
            return await _service.RunAsync(_request, _response, _service.UpdateAsync);
        }

        [HttpDelete(AccessTokenService.Route)]
        public async Task<ActionResult> Delete()
        {
            return await _service.RunAsync(_request, _response, _service.DeleteAsync);
        }
    }
}
