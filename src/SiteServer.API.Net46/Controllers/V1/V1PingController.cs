using SiteServer.API.Common;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/ping")]
    public class V1PingController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent("pong", Encoding.UTF8);

            return response;
        }
    }
}
