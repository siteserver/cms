using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ProfileController
    {
        [HttpPost, Route(RouteSendSms)]
        public async Task<ActionResult<BoolResult>> SendSms([FromBody] SendSmsRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Mobile))
            {
                return this.Error("请输入有效的手机号码");
            }

            var mobile = request.Mobile.Trim();
            if (!SmsRateLimitUtils.TryConsume(_cacheManager, typeof(ProfileController), mobile, PageUtils.GetIpAddress(Request), out var retryAfterSeconds))
            {
                return this.Error($"请求过于频繁，请在{retryAfterSeconds}秒后重试");
            }

            var user = await _authManager.GetUserAsync();
            if (!StringUtils.EqualsIgnoreCase(user.Mobile, mobile))
            {
                var exists = await _userRepository.IsMobileExistsAsync(mobile);
                if (exists)
                {
                    return this.Error("此手机号码已注册，请更换手机号码");
                }
            }

            var code = StringUtils.GetRandomInt(100000, 999999);
            var (success, errorMessage) =
                await _smsManager.SendSmsAsync(mobile, SmsCodeType.InformationChanges, code);
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
