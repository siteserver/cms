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
            var user = await _authManager.GetUserAsync();
            if (!StringUtils.EqualsIgnoreCase(user.Mobile, request.Mobile))
            {
                var exists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                if (exists)
                {
                    return this.Error("此手机号码已注册，请更换手机号码");
                }
            }

            var code = StringUtils.GetRandomInt(100000, 999999);
            var (success, errorMessage) =
                await _smsManager.SendAsync(request.Mobile, SmsCodeType.InformationChanges, code);
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
