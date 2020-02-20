using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SiteServer.Abstractions;

namespace SiteServer.Web.Controllers.V1
{
    [Route("v1/ping2")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;

        public PingController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        [OpenApiOperation("Ping 可用性 API", "Ping用于确定 API 是否可以访问，使用GET发起请求，请求地址为/api/v1/ping")]
        [HttpGet, Route(Route)]
        public string Get()
        {
            return "pong2";
        }
    }
}