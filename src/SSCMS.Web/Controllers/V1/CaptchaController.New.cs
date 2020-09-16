using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("生成验证码 API", "生成验证码，使用POST发起请求，请求地址为/api/v1/captcha")]
        [HttpPost, Route(Route)]
        public ActionResult<StringResult> New()
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
    }
}
