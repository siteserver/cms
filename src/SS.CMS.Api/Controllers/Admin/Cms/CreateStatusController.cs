using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.Admin.Cms;

namespace SS.CMS.Api.Controllers.Admin.Cms
{
    [Route(AdminRoutes.Prefix)]
    public class CreateStatusController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly CreateStatusService _service;

        public CreateStatusController(Request request, Response response, CreateStatusService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        [HttpGet(CreateStatusService.Route)]
        public ActionResult Get()
        {
            return _service.Run(_request, _response, _service.Get);
        }

        [HttpPost(CreateStatusService.RouteActionsCancel)]
        public ActionResult Cancel()
        {
            return _service.Run(_request, _response, _service.Cancel);
        }
    }
}