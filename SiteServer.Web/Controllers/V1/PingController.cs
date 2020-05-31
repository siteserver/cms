using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/ping")]
    public class PingController : ApiController
    {
        private const string Route = "";

        [OpenApiOperation("Ping 可用性 API", "https://sscms.com/docs/v6/api/guide/other/ping.html")]
        [HttpGet, Route(Route)]
        public HttpResponseMessage Get()
        {
            DataProvider.ChannelDao.UpdateWholeTaxisBySiteId(272);

            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent("pong", Encoding.UTF8);

            return response;
        }
    }
}
