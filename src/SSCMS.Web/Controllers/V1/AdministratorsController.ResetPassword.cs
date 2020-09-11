using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("修改管理员密码 API", "修改管理员密码，使用POST发起请求，请求地址为/api/v1/administrators/actions/resetPassword。")]
        [HttpPost, Route(RouteActionsResetPassword)]
        public async Task<ActionResult<Administrator>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }

            var (administrator, _, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);
            if (administrator == null)
            {
                return this.Error(errorMessage);
            }

            bool isValid;
            (isValid, errorMessage) = await _administratorRepository.ChangePasswordAsync(administrator, request.NewPassword);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return administrator;
        }
    }
}
