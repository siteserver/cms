using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("获取验证码图片 API", "获取验证码图片，使用GET发起请求，请求地址为/api/v1/captcha/{value}，此接口可以直接访问，无需身份验证。")]
        [HttpGet, Route(RouteValue)]
        public FileResult Get([FromRoute] string value)
        {
            var info = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(value));

            var bytes = CaptchaUtils.GetCaptcha(info.Value);

            return File(bytes, "image/png");
        }
    }
}
