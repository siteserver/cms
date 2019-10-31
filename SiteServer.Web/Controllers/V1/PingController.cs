using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using NSwag.Annotations;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/ping")]
    public class PingController : ApiController
    {
        private const string Route = "";

        [OpenApiOperation("Ping 可用性 API", "Ping用于确定 API 是否可以访问，使用GET发起请求，请求地址为/api/v1/ping")]
        [HttpGet, Route(Route)]
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent("pong", Encoding.UTF8);

            return response;
        }
    }
}
