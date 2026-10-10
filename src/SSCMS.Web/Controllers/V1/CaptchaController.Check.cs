using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("验证验证码 API", "验证验证码，使用POST发起请求，请求地址为/api/v1/captcha/actions/check，此接口可以直接访问，无需身份验证。")]
        [HttpPost, Route(RouteActionsCheck)]
        public ActionResult<BoolResult> Check([FromBody] CheckRequest request)
        {
            if (!CaptchaUtils.Validate(_cacheManager, request?.Value, request?.Captcha))
            {
                return this.Error(CaptchaUtils.ErrorInvalid);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
