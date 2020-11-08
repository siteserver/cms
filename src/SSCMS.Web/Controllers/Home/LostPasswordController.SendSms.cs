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
            var user = await _userRepository.GetByMobileAsync(request.Mobile);

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
                await _smsManager.SendAsync(request.Mobile, SmsCodeType.ChangePassword, code);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var cacheKey = GetSmsCodeCacheKey(request.Mobile);
            _cacheManager.AddOrUpdateAbsolute(cacheKey, code, 10);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
