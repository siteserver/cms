using System;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    [Route("v1/captcha")]
    public partial class CaptchaController : ControllerBase
    {
        private const string ApiRoute = "{name}";
        private const string ApiRouteActionsCheck = "{name}/actions/check";

        private readonly ICacheManager<bool> _cacheManager;

        public CaptchaController(ICacheManager<bool> cacheManager)
        {
            _cacheManager = cacheManager;
        }

        [HttpGet, Route(ApiRoute)]
        public FileResult Get(string name)
        {
            var (code, bytes) = ImageManager.GetCaptcha();

            var cookieName = "SS-" + name;

            Response.Cookies.Delete(cookieName);
            Response.Cookies.Append(cookieName, code, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10)
            });

            return File(bytes, "image/png");
        }

        [HttpPost, Route(ApiRouteActionsCheck)]
        public ActionResult<BoolResult> Check(string name, [FromBody] CheckRequest checkRequest)
        {
            var cookieName = "SS-" + name;

            if (!HttpContext.Request.Cookies.TryGetValue(cookieName, out var code))
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            var cacheKey = $"{cookieName}:{code}";

            if (_cacheManager.Exists(cacheKey))
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            HttpContext.Response.Cookies.Delete(Constants.AuthKeyAdminCaptchaCookie);

            if (!StringUtils.EqualsIgnoreCase(code, checkRequest.Captcha))
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
