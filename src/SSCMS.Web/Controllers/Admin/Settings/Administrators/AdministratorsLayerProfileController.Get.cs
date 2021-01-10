using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var adminName = _authManager.AdminName;
            if (adminName != request.UserName &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var administrator = await _administratorRepository.GetByUserNameAsync(request.UserName);

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
