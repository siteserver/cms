using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.Admin;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route(AdminRoutes.Prefix)]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly IndexService _service;

        public IndexController(Request request, Response response, IndexService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        [HttpGet(IndexService.Route)]
        public ActionResult Get()
        {
            return _service.Run(_request, _response, _service.Get);
        }

        [HttpGet(IndexService.RouteUnCheckedList)]
        public ActionResult GetUnCheckedList()
        {
            return _service.Run(_request, _response, _service.GetUnCheckedList);
        }
    }
}