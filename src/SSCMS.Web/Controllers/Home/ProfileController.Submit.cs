using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ProfileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] User request)
        {
            if (request.Id != _authManager.UserId) return Unauthorized();

            var (success, errorMessage) = await _userRepository.UpdateAsync(request);
            if (!success)
            {
                return this.Error($"修改资料失败：{errorMessage}");
            }

            await _authManager.AddUserLogAsync("修改资料");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
