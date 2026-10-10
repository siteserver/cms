using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var administrator = await _administratorRepository.GetByUserNameAsync(request.UserName);
            if (administrator == null) return this.Error(Constants.ErrorNotFound);

            // 越权校验：只能查看本人或有权管理的管理员
            if (administrator.Id != _authManager.AdminId &&
                !await _authManager.CanManageAdministratorAsync(administrator))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                UserId = administrator.Id,
                UserName = administrator.UserName,
                DisplayName = administrator.DisplayName,
                AvatarUrl = administrator.AvatarUrl,
                Mobile = administrator.Mobile,
                Email = administrator.Email
            };
        }
    }
}
