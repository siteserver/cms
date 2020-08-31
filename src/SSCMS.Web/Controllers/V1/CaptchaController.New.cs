using System;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
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
    }
}
