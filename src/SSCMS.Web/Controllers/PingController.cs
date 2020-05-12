using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers
{
    [Route("api/ping")]
    public class PingController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly ITaskManager _taskManager;

        public PingController(ISettingsManager settingsManager, ITaskManager taskManager)
        {
            _settingsManager = settingsManager;
            _taskManager = taskManager;
        }

        [OpenApiOperation("Ping 可用性 API", "Ping用于确定 API 是否可以访问，使用GET发起请求，请求地址为/api/ping")]
        [HttpGet, Route(Route)]
        public string Get()
        {
            _taskManager.Queue(async token =>
            {
                var filePath = PathUtils.Combine(_settingsManager.WebRootPath, "test.txt");
                await FileUtils.WriteTextAsync(filePath, "my name");
            });

            return "pong";
        }
    }
}