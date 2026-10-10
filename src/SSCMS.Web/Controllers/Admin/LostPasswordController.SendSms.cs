using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LostPasswordController
    {
        [HttpPost, Route(RouteSendSms)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BoolResult>> SendSms([FromBody] SendSmsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error("请输入手机号码");

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (allowed, limitMessage) = SmsCodeGuard.TryBeginSend(_cacheManager, codeKey, PageUtils.GetIpAddress(Request));
            if (!allowed) return this.Error(limitMessage);

            // 无论手机号是否绑定管理员、是否锁定，均返回相同结果，避免枚举（GJ20260921）
            var administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);
            if (administrator != null)
            {
                var (success, _) = await _administratorRepository.ValidateLockAsync(administrator);
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
