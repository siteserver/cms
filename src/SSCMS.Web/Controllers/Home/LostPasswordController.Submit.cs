using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LostPasswordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error(SmsCodeGuard.ErrorInvalidCode);

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, request.Code, PageUtils.GetIpAddress(Request), false);
            if (!isCodeValid)
            {
                return this.Error(codeError);
            }

            var user = await _userRepository.GetByMobileAsync(request.Mobile);
            if (user == null)
            {
                return this.Error(SmsCodeGuard.ErrorInvalidCode);
            }

            var password = request.Password;
            var (isValid, errorMessage) = await _userRepository.ChangePasswordAsync(user.Id, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            SmsCodeGuard.Invalidate(_cacheManager, codeKey);

            if (!user.MobileVerified)
            {
                user.MobileVerified = true;
                await _userRepository.UpdateAsync(user);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
