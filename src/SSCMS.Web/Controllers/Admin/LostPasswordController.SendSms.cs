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
            var administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);

            if (administrator == null)
            {
                return this.Error("此手机号码未关联管理员，请更换手机号码");
            }

            var (success, errorMessage) = await _administratorRepository.ValidateLockAsync(administrator);
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
