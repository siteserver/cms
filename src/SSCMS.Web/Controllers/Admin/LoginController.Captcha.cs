using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpGet, Route(RouteCaptcha)]
        public FileResult GetCaptcha()
        {
            var (code, bytes) = ImageManager.GetCaptcha();

            HttpContext.Response.Cookies.Delete(Constants.AuthKeyAdminCaptchaCookie);
            HttpContext.Response.Cookies.Append(Constants.AuthKeyAdminCaptchaCookie, code, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10)
            });

            return File(bytes, "image/png");
        }

        [HttpPost, Route(RouteCaptcha)]
        public ActionResult<BoolResult> Check([FromBody] CheckRequest captchaInfo)
        {
            if (!HttpContext.Request.Cookies.TryGetValue(Constants.AuthKeyAdminCaptchaCookie, out var code))
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            var cacheKey = $"{Constants.AuthKeyAdminCaptchaCookie}:{code}";

            if (_cacheManager.Exists(cacheKey))
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            HttpContext.Response.Cookies.Delete(Constants.AuthKeyAdminCaptchaCookie);

            if (!StringUtils.EqualsIgnoreCase(code, captchaInfo.Captcha))
            {
                return this.Error("验证码不正确，请重新输入！");
            }

            _cacheManager.Put(cacheKey, true);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
