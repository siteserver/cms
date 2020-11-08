using System;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpPost, Route(RouteCheckCaptcha)]
        public ActionResult<BoolResult> CaptchaCheck([FromBody] CheckRequest request)
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
