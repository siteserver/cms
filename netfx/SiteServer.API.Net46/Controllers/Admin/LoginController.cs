using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Services;
using SiteServer.CMS.Services.Admin;

namespace SiteServer.API.Controllers.Admin
{
    [RoutePrefix(AdminRoutes.Prefix)]
    public class LoginController : ControllerBase
    {
        [HttpGet, Route(LoginService.Route)]
        public IHttpActionResult Get()
        {
            return Run(ServiceProvider.LoginService.Get);
        }

        [HttpGet, Route(LoginService.RouteCaptcha)]
        public void GetCaptcha()
        {
            Run(ServiceProvider.LoginService.GetCaptcha);
        }

        [HttpPost, Route(LoginService.Route)]
        public IHttpActionResult Login()
        {
            return Run(ServiceProvider.LoginService.Login);
        }
    }
}