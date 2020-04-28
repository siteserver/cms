using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    [Authorize(Roles = AuthTypes.Roles.Api)]
    [Route(Constants.ApiV1Prefix)]
    public partial class CaptchaController : ControllerBase
    {
        private const string Route = "captcha";
        private const string RouteActionsCheck = "captcha/actions/check";

        private readonly ISettingsManager _settingsManager;

        public CaptchaController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        [HttpPost, Route(Route)]
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

        [HttpGet, Route(Route)]
        public FileResult Get([FromQuery]string captcha)
        {
            var info = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(captcha));

            var bytes = CaptchaUtils.GetCaptcha(info.Value);

            return File(bytes, "image/png");
        }

        [HttpPost, Route(RouteActionsCheck)]
        public ActionResult<BoolResult> Check(string name, [FromBody] CheckRequest request)
        {
            var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(request.Captcha));

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
