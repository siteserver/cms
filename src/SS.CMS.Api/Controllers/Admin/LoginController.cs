using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Core.Services.Admin;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route(AdminRoutes.Prefix)]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Request _request;
        private readonly Response _response;
        private readonly LoginService _service;

        public LoginController(Request request, Response response, LoginService service)
        {
            _request = request;
            _response = response;
            _service = service;
        }

        [HttpGet(LoginService.Route)]
        public ActionResult Get()
        {
            return _service.Run(_request, _response, _service.Get);
        }

        [HttpGet(LoginService.RouteCaptcha)]
        public ActionResult GetCaptcha()
        {
            return _service.Run(_request, _response, _service.GetCaptcha);
        }

        [HttpPost(LoginService.Route)]
        public ActionResult Login()
        {
            return _service.Run(_request, _response, _service.Login);
        }
    }
}