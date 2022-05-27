using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerPasswordController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var userName = request.UserName;
            var adminName = _authManager.AdminName;

            if (string.IsNullOrEmpty(userName)) userName = adminName;
            var administrator = await _administratorRepository.GetByUserNameAsync(userName);
            if (administrator == null) return this.Error(Constants.ErrorNotFound);
            if (userName != adminName &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
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
