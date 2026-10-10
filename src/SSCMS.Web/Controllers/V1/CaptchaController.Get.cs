using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("获取验证码图片 API", "获取验证码图片，使用GET发起请求，请求地址为/api/v1/captcha/{value}，此接口可以直接访问，无需身份验证。")]
        [HttpGet, Route(RouteValue)]
        public ActionResult Get([FromRoute] string value)
        {
            var code = CaptchaUtils.GetValue(_cacheManager, value);
            if (code == null) return NotFound();

            var bytes = CaptchaUtils.GetCaptcha(code);
            return File(bytes, "image/png");
        }
    }
}
