using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class VerifyMobileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
            var code = _cacheManager.Get<int>(codeCacheKey);
            if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
            {
                return this.Error("输入的验证码有误或验证码已超时");
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
