using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace SSCMS.Web.Controllers
{
    public partial class PingController
    {
        [OpenApiOperation("Ping 可用性 API", "Ping用于确定 API 是否可以访问，使用GET发起请求，请求地址为/api/ping，此接口可以直接访问，无需身份验证。")]
        [HttpGet, Route(Route)]
        public string Get()
        {
            //_taskManager.Queue(async cancel =>
            //{
            //    var filePath = PathUtils.Combine(_settingsManager.WebRootPath, "test.txt");
            //    await FileUtils.WriteTextAsync(filePath, "my name");
            //});

            //_taskManager.RunOnceAt(async () =>
            //{
            //    var filePath = PathUtils.Combine(_settingsManager.WebRootPath, "test.txt");
            //    await FileUtils.WriteTextAsync(filePath, DateTime.Now.ToString(CultureInfo.InvariantCulture));
            //}, DateTime.Now.AddSeconds(10));

            return "pong";
        }
    }
}