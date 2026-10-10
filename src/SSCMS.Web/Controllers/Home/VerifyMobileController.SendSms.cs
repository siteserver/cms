using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class VerifyMobileController
    {
        [HttpPost, Route(RouteSendSms)]
        public async Task<ActionResult<BoolResult>> SendSms([FromBody] SendSmsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error("请输入手机号码");

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (allowed, limitMessage) = SmsCodeGuard.TryBeginSend(_cacheManager, codeKey, PageUtils.GetIpAddress(Request));
            if (!allowed) return this.Error(limitMessage);

            var user = await _authManager.GetUserAsync();
            if (!StringUtils.EqualsIgnoreCase(user.Mobile, request.Mobile))
            {
                var exists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                if (exists)
                {
                    return this.Error("此手机号码已注册，请更换手机号码");
                }
            }

            var code = SmsCodeGuard.NewCode();
            var (success, _) =
                await _smsManager.SendSmsAsync(request.Mobile, SmsCodeType.Authentication, code);
            if (!success)
            {
                return this.Error(SmsCodeGuard.ErrorSendFailed);
            }

            SmsCodeGuard.SaveCode(_cacheManager, codeKey, code);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
