using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.Admin;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route(AdminRoutes.Prefix)]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly SyncService _service;

        public SyncController(Request request, Response response, SyncService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        [HttpGet(SyncService.Route)]
        public ActionResult Get()
        {
            return _service.Run(_request, _response, _service.Get);
        }

        [HttpPost(SyncService.Route)]
        public ActionResult Update()
        {
            return _service.Run(_request, _response, _service.Update);
        }
    }
}