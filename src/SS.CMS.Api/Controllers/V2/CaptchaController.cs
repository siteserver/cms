using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.V2;

namespace SS.CMS.Api.Controllers.V2
{
    [Route(V2Routes.Prefix)]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly CaptchaService _service;

        public CaptchaController(Request request, Response response, CaptchaService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        [HttpGet(CaptchaService.Route)]
        public ActionResult Get(string name)
        {
            return _service.Run(_request, _response, _service.Get);
        }

        [HttpPost(CaptchaService.RouteActionsCheck)]
        public ActionResult Check(string name)
        {
            return _service.Run(_request, _response, _service.ActionsCheck);
        }
    }
}
