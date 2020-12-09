using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersStyleController
    {
        [HttpGet, Route(RouteExport)]
        public async Task<ActionResult> Export()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(_pathManager, _databaseManager, 0, _userRepository.TableName, _tableStyleRepository.EmptyRelatedIdentities);

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            return this.Download(filePath);
        }
    }
}
