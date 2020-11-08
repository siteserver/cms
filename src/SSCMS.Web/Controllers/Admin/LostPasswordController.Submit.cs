using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
            var code = _cacheManager.Get<int>(codeCacheKey);
            if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
            {
                return this.Error("输入的验证码有误或验证码已超时，请重试");
            }

            var administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);

            if (administrator == null)
            {
                return this.Error("此手机号码未关联管理员，请更换手机号码");
            }

            var password = request.Password;
            var (isValid, errorMessage) = await _administratorRepository.ChangePasswordAsync(administrator, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            await _authManager.AddAdminLogAsync("重设管理员密码", $"管理员:{administrator.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
