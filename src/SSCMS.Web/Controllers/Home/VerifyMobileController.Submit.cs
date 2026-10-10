using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class VerifyMobileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error(SmsCodeGuard.ErrorInvalidCode);

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, request.Code, PageUtils.GetIpAddress(Request));
            if (!isCodeValid)
            {
                return this.Error(codeError);
            }

            var user = await _authManager.GetUserAsync();

            user.Mobile = request.Mobile;
            user.MobileVerified = true;
            await _userRepository.UpdateAsync(user);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
