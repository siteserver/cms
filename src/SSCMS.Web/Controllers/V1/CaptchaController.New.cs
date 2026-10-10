using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.V1
{
    public partial class CaptchaController
    {
        [OpenApiOperation("生成验证码 API", "生成验证码，使用POST发起请求，请求地址为/api/v1/captcha，此接口可以直接访问，无需身份验证。")]
        [HttpPost, Route(Route)]
        public ActionResult<StringResult> New()
        {
            return new StringResult
            {
                Value = CaptchaUtils.Issue(_cacheManager)
            };
        }
    }
}
