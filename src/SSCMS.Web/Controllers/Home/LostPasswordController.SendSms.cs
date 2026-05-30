using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            if (request == null || string.IsNullOrWhiteSpace(request.Mobile))
            {
                return this.Error("请输入有效的手机号码");
            }

            var mobile = request.Mobile.Trim();
            if (!SmsRateLimitUtils.TryConsume(_cacheManager, typeof(LostPasswordController), mobile, PageUtils.GetIpAddress(Request), out var retryAfterSeconds))
            {
                return this.Error($"请求过于频繁，请在{retryAfterSeconds}秒后重试");
            }

            var user = await _userRepository.GetByMobileAsync(mobile);

            if (user == null)
            {
                return this.Error("此手机号码未关联用户");
            }

            var (success, errorMessage) = await _userRepository.ValidateStateAsync(user);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var code = StringUtils.GetRandomInt(100000, 999999);
            (success, errorMessage) =
                await _smsManager.SendSmsAsync(mobile, SmsCodeType.ChangePassword, code);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var cacheKey = GetSmsCodeCacheKey(mobile);
            _cacheManager.AddOrUpdateAbsolute(cacheKey, code, 10);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
