using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LoginController
    {
        [HttpGet, Route(RouteCaptcha)]
        public ActionResult CaptchaGet([FromQuery] string token)
        {
            var value = CaptchaUtils.GetValue(_cacheManager, token);
            if (value == null) return NotFound();

            var bytes = CaptchaUtils.GetCaptcha(value);
            return File(bytes, "image/png");
        }
    }
}
