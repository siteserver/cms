using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-3.0

namespace SiteServer.API.Controllers
{
    [Route("api/features")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly ApplicationPartManager _partManager;

        public FeaturesController(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }

        private const string Route = "";

        [HttpGet(Route)]
        public ActionResult Index()
        {
            var controllerFeature = new ControllerFeature();
            _partManager.PopulateFeature(controllerFeature);
            var controllers = controllerFeature.Controllers.ToList();

            return Ok(new
            {
                Value = controllers
            });
        }
    }
}