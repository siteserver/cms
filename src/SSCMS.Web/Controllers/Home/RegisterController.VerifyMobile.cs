using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(RouteVerifyMobile)]
        public ActionResult<BoolResult> VerifyMobile([FromBody] VerifyMobileRequest request)
        {
            var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
            var code = _cacheManager.Get<int>(codeCacheKey);
            if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
            {
                return this.Error("输入的验证码有误或验证码已超时");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
