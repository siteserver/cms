using System;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(RouteCaptcha)]
        public StringResult New()
        {
            var captcha = new CaptchaUtils.Captcha
            {
                Value = CaptchaUtils.GetCode(),
                ExpireAt = DateTime.Now.AddMinutes(10)
            };
            var json = TranslateUtils.JsonSerialize(captcha);

            return new StringResult
            {
                Value = _settingsManager.Encrypt(json)
            };
        }

        [HttpGet, Route(RouteCaptcha)]
        public FileResult Get([FromQuery] string token)
        {
            var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(token));

            var bytes = CaptchaUtils.GetCaptcha(captcha.Value);

            return File(bytes, "image/png");
        }

        [HttpPost, Route(RouteCheckCaptcha)]
        public ActionResult<BoolResult> Check([FromBody] CheckRequest request)
        {
            var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(request.Token));

            if (captcha == null || string.IsNullOrEmpty(captcha.Value) || captcha.ExpireAt < DateTime.Now)
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            if (!StringUtils.EqualsIgnoreCase(captcha.Value, request.Value))
            {
                return this.Error("验证码不正确，请重新输入！");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
