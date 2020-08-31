using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("获取验证码图片 API", "获取验证码图片，使用GET发起请求，请求地址为/api/v1/captcha，此接口可以直接访问，无需身份验证。")]
        [HttpGet, Route(Route)]
        public FileResult Get([FromQuery] string captcha)
        {
            var info = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(captcha));

            var bytes = CaptchaUtils.GetCaptcha(info.Value);

            return File(bytes, "image/png");
        }
    }
}
