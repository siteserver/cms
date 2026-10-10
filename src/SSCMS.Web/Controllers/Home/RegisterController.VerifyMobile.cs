using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(RouteVerifyMobile)]
        public ActionResult<BoolResult> VerifyMobile([FromBody] VerifyMobileRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error(SmsCodeGuard.ErrorInvalidCode);

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, request.Code, PageUtils.GetIpAddress(Request));
            if (!isCodeValid)
            {
                return this.Error(codeError);
            }

            // 记录该手机号已通过验证，注册提交时消费，防止跳过验证直接注册
            SmsCodeGuard.MarkVerified(_cacheManager, codeKey);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
