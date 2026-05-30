using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            if (request == null || string.IsNullOrWhiteSpace(request.Mobile))
            {
                return this.Error("请输入有效的手机号码");
            }

            var mobile = request.Mobile.Trim();
            if (!TryConsumeSendSmsQuota(mobile, PageUtils.GetIpAddress(Request), out var retryAfterSeconds))
            {
                return this.Error($"请求过于频繁，请在{retryAfterSeconds}秒后重试");
            }

            var administrator = await _administratorRepository.GetByMobileAsync(mobile);
            if (administrator != null)
            {
                var (success, _) = await _administratorRepository.ValidateLockAsync(administrator);
                if (success)
                {
                    var code = StringUtils.GetRandomInt(100000, 999999);
                    (success, _) = await _smsManager.SendSmsAsync(mobile, SmsCodeType.ChangePassword, code);
                    if (success)
                    {
                        var cacheKey = GetSmsCodeCacheKey(mobile);
                        _cacheManager.AddOrUpdateAbsolute(cacheKey, code, 10);
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
