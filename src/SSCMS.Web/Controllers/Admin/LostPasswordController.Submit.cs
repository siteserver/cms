using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LostPasswordController
    {
        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error(SmsCodeGuard.ErrorInvalidCode);

            var codeKey = GetSmsCodeCacheKey(request.Mobile);
            var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, request.Code, PageUtils.GetIpAddress(Request), false);
            if (!isCodeValid)
            {
                return this.Error(codeError);
            }

            var administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);
            if (administrator == null)
            {
                return this.Error(SmsCodeGuard.ErrorInvalidCode);
            }

            var password = request.Password;
            var (isValid, errorMessage) = await _administratorRepository.ChangePasswordAsync(administrator, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            SmsCodeGuard.Invalidate(_cacheManager, codeKey);
            await _authManager.AddAdminLogAsync("重设管理员密码", $"管理员:{administrator.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
