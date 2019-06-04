using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.Admin;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route(AdminRoutes.Prefix)]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly RootService _service;

        public RootController(Request request, Response response, RootService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        [HttpGet(RootService.Route)]
        public ActionResult Get()
        {
            return _service.Run(_request, _response, _service.GetConfig);
        }

        [HttpPost(RootService.Route)]
        public async Task<ActionResult> Create()
        {
            return await _service.Run(_request, _response, _service.Create);
        }

        [HttpPost(RootService.RouteActionsDownload)]
        public ActionResult Download()
        {
            return _service.Run(_request, _response, _service.Download);
        }
    }
}