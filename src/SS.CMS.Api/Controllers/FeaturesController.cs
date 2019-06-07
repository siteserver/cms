using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-3.0

namespace SS.CMS.Api.Controllers
{
    [Route("features")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly ApplicationPartManager _partManager;

        public FeaturesController(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }

        private const string Route = "{name}";

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="name">name of action</param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(201)]     // Created
        [ProducesResponseType(400)]     // BadRequest
        [HttpGet(Route), ActionName("Get Index")]
        public ActionResult Index(string name)
        {
            var controllerFeature = new ControllerFeature();
            _partManager.PopulateFeature(controllerFeature);
            var controllers = controllerFeature.Controllers.ToList();

            return Ok(new
            {
                Value = controllers,
                Request.RouteValues
            });
        }
    }
}