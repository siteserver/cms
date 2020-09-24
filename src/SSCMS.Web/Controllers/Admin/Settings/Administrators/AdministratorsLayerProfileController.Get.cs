using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int userId)
        {
            var adminId = _authManager.AdminId;
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var administrator = await _administratorRepository.GetByUserIdAsync(userId);

            return new GetResult
            {
                UserName = administrator.UserName,
                DisplayName = administrator.DisplayName,
                AvatarUrl = administrator.AvatarUrl,
                Mobile = administrator.Mobile,
                Email = administrator.Email
            };
        }
    }
}
