using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerPasswordController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var userId = request.UserId;
            var adminId = _authManager.AdminId;

            if (userId == 0) userId = adminId;
            var administrator = await _administratorRepository.GetByUserIdAsync(userId);
            if (administrator == null) return NotFound();
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Administrator = administrator
            };
        }
    }
}
