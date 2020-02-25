using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace SS.CMS.Web.Controllers
{
    [Route("ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private const string Route = "";

        [OpenApiOperation("Ping 可用性 API", "Ping用于确定 API 是否可以访问，使用GET发起请求，请求地址为/api/ping")]
        [HttpGet, Route(Route)]
        public string Get()
        {
            return "pong";
        }
    }
}