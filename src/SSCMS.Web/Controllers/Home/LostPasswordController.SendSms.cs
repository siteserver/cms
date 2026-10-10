using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LostPasswordController
    {
        [HttpPost, Route(RouteSendSms)]
        public async Task<ActionResult<BoolResult>> SendSms([FromBody] SendSmsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error("请输入手机号码");

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (allowed, limitMessage) = SmsCodeGuard.TryBeginSend(_cacheManager, codeKey, PageUtils.GetIpAddress(Request));
            if (!allowed) return this.Error(limitMessage);

            // 无论手机号是否绑定用户，均返回相同结果，避免枚举
            var user = await _userRepository.GetByMobileAsync(request.Mobile);
            if (user != null)
            {
                var (success, _) = await _userRepository.ValidateStateAsync(user);
                if (success)
                {
                    var code = SmsCodeGuard.NewCode();
                    (success, _) = await _smsManager.SendSmsAsync(request.Mobile, SmsCodeType.ChangePassword, code);
                    if (success)
                    {
                        SmsCodeGuard.SaveCode(_cacheManager, codeKey, code);
                    }
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
