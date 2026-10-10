using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(RouteCheckCaptcha)]
        public ActionResult<BoolResult> CaptchaCheck([FromBody] CheckRequest request)
        {
            if (!CaptchaUtils.Validate(_cacheManager, request?.Token, request?.Value))
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
