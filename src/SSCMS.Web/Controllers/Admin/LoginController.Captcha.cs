using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
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

        //[HttpGet, Route(RouteCaptcha)]
        //public FileResult GetCaptcha()
        //{
        //    var (code, bytes) = ImageManager.GetCaptcha();

        //    HttpContext.Response.Cookies.Delete(Constants.AuthKeyAdminCaptchaCookie);
        //    HttpContext.Response.Cookies.Append(Constants.AuthKeyAdminCaptchaCookie, code, new CookieOptions
        //    {
        //        Expires = DateTime.Now.AddMinutes(10)
        //    });

        //    return File(bytes, "image/png");
        //}

        //[HttpPost, Route(RouteCheckCaptcha)]
        //public ActionResult<BoolResult> Check([FromBody] CheckRequest captchaInfo)
        //{
        //    if (!HttpContext.Request.Cookies.TryGetValue(Constants.AuthKeyAdminCaptchaCookie, out var code))
        //    {
        //        return this.Error("验证码已超时，请点击刷新验证码！");
        //    }

        //    var cacheKey = $"{Constants.AuthKeyAdminCaptchaCookie}:{code}";

        //    if (_cacheManager.Exists(cacheKey))
        //    {
        //        return this.Error("验证码已超时，请点击刷新验证码！");
        //    }

        //    HttpContext.Response.Cookies.Delete(Constants.AuthKeyAdminCaptchaCookie);

        //    if (!StringUtils.EqualsIgnoreCase(code, captchaInfo.Captcha))
        //    {
        //        return this.Error("验证码不正确，请重新输入！");
        //    }

        //    _cacheManager.Put(cacheKey, true);

        //    return new BoolResult
        //    {
        //        Value = true
        //    };
        //}
    }
}
