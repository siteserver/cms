using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpGet, Route(RouteCaptcha)]
        public FileResult CaptchaGet([FromQuery] string token)
        {
            var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(token));

            var bytes = CaptchaUtils.GetCaptcha(captcha.Value);

            return File(bytes, "image/png");
        }
    }
}
